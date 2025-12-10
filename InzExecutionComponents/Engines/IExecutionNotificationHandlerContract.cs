namespace InzExecutionComponents.Engines;

internal interface IExecutionNotificationHandlerContract
{
    public string Name { get; set; }
    public string RegistrationKey { get; set; }
    public Type ImplementationType { get; set; }
    public string ImplementationTypeId { get; set; }
}