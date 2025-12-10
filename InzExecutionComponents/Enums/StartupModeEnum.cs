namespace InzExecutionComponents.Enums;

[Flags]
public enum StartupModeEnum
{
    Initializer = 0,
    ApiServer = 1,
    EventHandler = 2,
    DirectDrive = 3
}