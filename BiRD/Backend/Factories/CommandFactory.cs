using BifrostRemoteDesktop.Common.Enums;
using BifrostRemoteDesktop.Common.Models.Commands;
using BifrostRemoteDesktop.Common.SystemControllers;
using BifrostRemoteDesktop.WPF.Backend.Models.Commands;
using BiRD.Backend.Models.Commands;
using System;

namespace BifrostRemoteDesktop.Common.Factories
{
    public static class CommandFactory
    {

        public static ICommand CreateCommand(CommandType commandType, RemoteControlCommandArgs commandArgs, ISystemController systemController)
        {
            switch (commandType)
            {
                case CommandType.MovePointer:
                    {
                        return new MovePointerCommand(systemController, (MovePointerCommandArgs)commandArgs);
                    }
                case CommandType.MouseAction:
                    {
                        return new MouseActionCommand(systemController, (MouseActionCommandArgs)commandArgs);
                    }
                case CommandType.MovePointerPercentage:
                    {
                        return new MovePointerPercentCommand(systemController, (MovePointerPercentageCommandArgs)commandArgs);
                    }
                case CommandType.KeyboardInputCommand:
                    {
                        return new KeyboardInputCommand(systemController, (KeyboardInputCommandArgs)commandArgs);
                    }
                case CommandType.ConnectionRequest:
                    {
                        return new ConnectionRequestCommand(systemController, (ConnectionRequestCommandArgs)commandArgs);
                    }
                default:
                    {
                        throw new ArgumentException($"The argument of parameter {nameof(commandType)} does not corrospond to a any known command.");
                    }
            }
            throw new ArgumentException($"The argument of parameter {nameof(commandType)} does not corrospond to a any known command.");
        }
    }
}
