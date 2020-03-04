namespace DEVICE_CORE.State.SubWorkflows.Providers
{
    internal interface IDeviceSubStateActionControllerProvider
    {
        IDeviceSubStateActionController GetStateActionController(IDeviceSubStateManager manager);
    }
}
