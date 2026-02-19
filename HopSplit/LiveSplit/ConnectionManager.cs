using System.Diagnostics;
using System;
using System.Text;
using System.Threading.Tasks;

namespace HopSplit.LiveSplit
{
    internal static class ConnectionManager
    {
        private const   int             BUFFER      = 1024;
        private const   string          TimeFormat  = @"hh\:mm\:ss\.fffffff";
        internal static bool            IsConnected => LiveSplitManager.Stream != null && LiveSplitManager.ConnectionStatus == LiveSplitManager.Status.Connected;

        private enum Commands
        {
            GetCurrentGameTime,
            SetGameTime,
            PauseGameTime,
            UnpauseGameTime,
            Reset,
            StartTimer,
            Split
        }

        internal static bool StartingNewGame = false;

        private static string GetCommand(Commands command, string parameter)
        {
            return parameter != null ? $"{command.ToString().ToLower()} {parameter}" : command.ToString().ToLower();
        }

        private static async Task<(int, byte[])> SendAndReceiveResponse(Commands command, string parameter = null)
        {
            string data = GetCommand(command, parameter);
            return await SendAndReceiveResponse(data);
        }

        private static async Task<bool> SendWithNoResponse(Commands command, string parameter = null)
        {
            string data = GetCommand(command, parameter);
            return await SendWithNoResponse(data);
        }

        private static async Task<(int, byte[])> SendAndReceiveResponse(string data)
        {
            byte[] commandBytes = Encoding.UTF8.GetBytes($"{data}\r\n");
            if (!(await WriteToStream(commandBytes)))
                return default;

            var response = await ReadCurrentStream();
            return response;
        }

        private static async Task<bool> SendWithNoResponse(string data)
        {
            byte[] commandBytes = Encoding.UTF8.GetBytes($"{data}\r\n");
            return await WriteToStream(commandBytes);
        }

        private static async Task<bool> WriteToStream(byte[] bytes, bool force = false)
        {
            if (IsConnected)
            {
                try
                {
                    await LiveSplitManager.Stream.WriteAsync(bytes, 0, bytes.Length);
                    await LiveSplitManager.Stream.FlushAsync();
                    return true;
                } catch { return false; }
            }
            return false;
        }

        private static async Task<(int, byte[])> ReadCurrentStream()
        {
            try
            {
                if (IsConnected && LiveSplitManager.Stream.DataAvailable)
                {
                    var responseBuffer = new byte[BUFFER];
                    return (await LiveSplitManager.Stream.ReadAsync(responseBuffer, 0, responseBuffer.Length), responseBuffer);
                }
            } catch { return default; }
            return default;
        }

        internal static async void StartAddingCutsceneTime(TimeSpan time)
        {
            if (IsConnected)
                await AddCutsceneTime(time);
        }

        private static async Task AddCutsceneTime(TimeSpan time)
        {
            Stopwatch responseTimer = Stopwatch.StartNew();
            var gameTimeResponse = await SendAndReceiveResponse(Commands.GetCurrentGameTime);
            responseTimer.Stop();

            if (gameTimeResponse == default)
                return;

            string response = Encoding.ASCII.GetString(gameTimeResponse.Item2, 0, gameTimeResponse.Item1).Trim();
            if (TimeSpan.TryParse(response, out TimeSpan currentGameTime))
            {
                TimeSpan newTime = (currentGameTime + time) - responseTimer.Elapsed;
                await SendWithNoResponse(Commands.SetGameTime, newTime.ToString(TimeFormat));
            }
        }

        internal static async void StartSettingGameTime(TimeSpan time)
        {
            if (IsConnected)
                await SetGameTime(time);
        }

        private static async Task SetGameTime(TimeSpan time)
        {
            await SendWithNoResponse(Commands.SetGameTime, time.ToString(TimeFormat));
        }

        internal static async void StartPausingTimer()
        {
            if (IsConnected)
                await PauseTimer();
        }

        private static async Task PauseTimer()
        {
            await SendWithNoResponse(Commands.PauseGameTime);
        }

        internal static async void StartUnpausingTimer()
        {
            if (IsConnected)
                await UnpauseTimer();
        }

        private static async Task UnpauseTimer()
        {
            if (StartingNewGame)
            {
                StartingNewGame = false;
                await StartNewGame();
            }

            await SendWithNoResponse(Commands.UnpauseGameTime);
        }

        internal static async void StartSplit()
        {
            if (IsConnected)
                await Split();
        }

        private static async Task Split()
        {
            await SendWithNoResponse(Commands.Split);
        }

        internal static async Task StartNewGame()
        {
            if (IsConnected)
                await NewGame();
        }

        private static async Task NewGame()
        {
            await SendWithNoResponse(Commands.Reset);
            await SendWithNoResponse(Commands.StartTimer);

            if (!ConfigHandler.ForceSyncTime)
                await SendWithNoResponse(Commands.PauseGameTime);
            else
                await SyncToGame();
        }

        internal static async void StartSyncingToGame()
        {
            if (IsConnected)
                await SyncToGame();
        }

        private static async Task SyncToGame()
        {
            var playerManager = SingletonPropertyItem<PlayerManager>.Instance;
            if (playerManager != null)
                await SetGameTime(TimeSpan.FromSeconds(playerManager.GetTotalPlaytimeInSeconds()));
        }

        // Debug -----------------------------------------------------------------------------------------------

        internal static async Task<TimeSpan> StartGettingGameTime()
        {
            if (IsConnected)
                return await GetGameTimeFull();
            return default(TimeSpan);
        }

        private static async Task<TimeSpan> GetGameTimeFull()
        {
            var gameTime = await SendAndReceiveResponse(Commands.GetCurrentGameTime);
            if (gameTime == default)
                return default;

            string response = Encoding.ASCII.GetString(gameTime.Item2, 0, gameTime.Item1).Trim();
            if (TimeSpan.TryParse(response, out TimeSpan currentGameTime))
                return currentGameTime;
            return default(TimeSpan);
        }
    }
}
