using HarvestReport.Utils;
using ManyConsole;
using NLog;
using System;
using System.Collections.Generic;

namespace HarvestReport
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            Logger logger = LogManager.LoadConfiguration("nlog.config")
                                      .GetCurrentClassLogger();

            try
            {
                IServiceProvider serviceProvider = Bootstrapper.Run();
                IEnumerable<ConsoleCommand> commands = ConsoleCommandDispatcher.FindCommandsInSameAssemblyAs(typeof(Program));
                var result = ConsoleCommandDispatcher.DispatchCommand(commands, args, Console.Out);

                return result;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Stopped program because of exception");
                return (int) ExitCodes.UnknownError;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private enum ExitCodes
        {
            UnknownError = -1,
            Ok
        }
    }
}
