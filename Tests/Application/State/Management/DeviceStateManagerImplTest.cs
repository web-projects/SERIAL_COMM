using Moq;
using Ninject;
using DEVICE_CORE.Config;
using DEVICE_CORE.Modules;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Actions.Controllers;
using DEVICE_CORE.StateMachine.State.Enums;
using DEVICE_CORE.StateMachine.State.Management;
using DEVICE_CORE.StateMachine.State.Providers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace DEVICE_CORE.Tests.State.Management
{
    public class DeviceStateManagerImplTest
    {
        readonly DeviceStateManagerImpl subject;

        readonly DeviceSection DeviceAppSection;
        readonly Mock<IDeviceConfigurationProvider> mockConfigurationProvider;

        //readonly Mock<IChannelClient> mockChannelClient;

        readonly Mock<IDeviceStateAction> mockDeviceStateAction;
        readonly Mock<IDeviceStateActionController> mockDeviceStateActionController;
        readonly Mock<IDeviceStateActionControllerProvider> mockDeviceStateActionControllerProvider;
        //readonly Mock<ILoggingServiceClient> mockLoggingServiceClient;
        //readonly Mock<ILoggingServiceClientProvider> mockLoggingServiceClientProvider;

        //readonly Mock<IListenerConnector> mockListenerConnector;
        //readonly Mock<IListenerConnectorProvider> mockListenerConnectorProvider;

        //readonly Mock<IDevicePluginLoader> mockDevicePluginLoader;
        //readonly List<ICardDevice> cardDevices;
        //readonly Mock<ICardDevice> fakeDeviceOne;
        //readonly Mock<ICardDevice> fakeDeviceTwo;

        const string someFakePath = @"C:\fakepluginpath";

        public DeviceStateManagerImplTest()
        {
            subject = new DeviceStateManagerImpl();

            DeviceAppSection = new DeviceSection();
            mockConfigurationProvider = new Mock<IDeviceConfigurationProvider>();
            mockConfigurationProvider.Setup(e => e.GetAppConfig()).Returns(DeviceAppSection);

            //mockChannelClient = new Mock<IChannelClient>();

            //mockListenerConnector = new Mock<IListenerConnector>();
            //mockListenerConnector.SetupAllProperties();
            //mockListenerConnector.Setup(e => e.ChannelClient).Returns(mockChannelClient.Object);

            //mockListenerConnectorProvider = new Mock<IListenerConnectorProvider>();
            //mockListenerConnectorProvider.Setup(e => e.GetConnector(It.IsAny<IConfiguration>())).Returns(mockListenerConnector.Object);

            //mockLoggingServiceClient = new Mock<ILoggingServiceClient>();

            //mockLoggingServiceClientProvider = new Mock<ILoggingServiceClientProvider>();
            //mockLoggingServiceClientProvider.Setup(e => e.GetLoggingServiceClient()).Returns(mockLoggingServiceClient.Object);

            mockDeviceStateAction = new Mock<IDeviceStateAction>();
            mockDeviceStateAction.Setup(e => e.DoWork()).Returns(Task.CompletedTask);

            mockDeviceStateActionController = new Mock<IDeviceStateActionController>();
            mockDeviceStateActionController.Setup(e => e.GetNextAction(It.IsAny<DeviceWorkflowState>())).Returns(mockDeviceStateAction.Object);
            mockDeviceStateActionController.Setup(e => e.GetFinalState()).Returns(mockDeviceStateAction.Object);

            mockDeviceStateActionControllerProvider = new Mock<IDeviceStateActionControllerProvider>();
            mockDeviceStateActionControllerProvider.Setup(e => e.GetStateActionController(subject)).Returns(mockDeviceStateActionController.Object);

            // Setup fake card devices list and also 2 fake devices for testing purposes.
            //cardDevices = new List<ICardDevice>();

            //fakeDeviceOne = new Mock<ICardDevice>();
            //fakeDeviceTwo = new Mock<ICardDevice>();

            //cardDevices.AddRange(new ICardDevice[] { fakeDeviceOne.Object, fakeDeviceTwo.Object });

            //mockDevicePluginLoader = new Mock<IDevicePluginLoader>();
            //mockDevicePluginLoader.Setup(e => e.FindAvailableDevices(someFakePath)).Returns(cardDevices);

            using IKernel kernel = new DeviceKernelResolver().ResolveKernel();
            //kernel.Rebind<IListenerConnectorProvider>().ToConstant(mockListenerConnectorProvider.Object);
            //kernel.Rebind<IDevicePluginLoader>().ToConstant(mockDevicePluginLoader.Object);
            kernel.Rebind<IDeviceConfigurationProvider>().ToConstant(mockConfigurationProvider.Object);
            //kernel.Rebind<ILoggingServiceClientProvider>().ToConstant(mockLoggingServiceClientProvider.Object);
            kernel.Rebind<IDeviceStateActionControllerProvider>().ToConstant(mockDeviceStateActionControllerProvider.Object);
            kernel.Bind<DeviceStateManagerImpl>().ToSelf();
            kernel.Inject(subject);
        }

        [Fact]
        public void Initialize_ShouldGrabReferencesToConcreteObjects_When_InjectionIsCompleted()
        {
            //Assert.Same(mockLoggingServiceClient.Object, subject.LoggingClient);

            IDeviceStateActionController controller = TestHelper.Helper.GetFieldValueFromInstance<IDeviceStateActionController>(
                "stateActionController", false, false, subject);

            Assert.Same(mockDeviceStateActionController.Object, controller);

            mockConfigurationProvider.Verify(e => e.InitializeConfiguration(), Times.Once());
            mockConfigurationProvider.Verify(e => e.GetAppConfig(), Times.Once());
            //mockLoggingServiceClientProvider.Verify(e => e.GetLoggingServiceClient(), Times.Once());
            mockDeviceStateActionControllerProvider.Verify(e => e.GetStateActionController(subject), Times.Once());
        }

        [Fact]
        public void LaunchWorkflow_ShouldInvokeCallToGetNextAction_When_Called()
        {
            subject.LaunchWorkflow();

            mockDeviceStateActionController.Verify(e => e.GetNextAction(DeviceWorkflowState.None), Times.Once());
            mockDeviceStateAction.Verify(e => e.DoWork(), Times.Once());
        }

        [Fact]
        public void StopWorkflow_ShouldCallNecessaryActions_ToCloseAppGracefully_When_Called()
        {
            subject.StopWorkflow();

            //mockLoggingServiceClient.Verify(e => e.LogInfoAsync("Currently shutting down Device Workflow...", It.IsAny<Dictionary<string, object>>()), Times.Once());

            mockDeviceStateActionController.Verify(e => e.GetFinalState(), Times.Once());

            mockDeviceStateAction.Verify(e => e.DoWork(), Times.Once());
        }

        [Fact]
        public void Dispose_ShouldCallStopWorkflow_When_Called()
        {
            subject.Dispose();

            //mockLoggingServiceClient.Verify(e => e.LogInfoAsync("Currently shutting down Device Workflow...", It.IsAny<Dictionary<string, object>>()), Times.Once());

            mockDeviceStateAction.Verify(e => e.Dispose(), Times.Once());

            mockDeviceStateActionController.Verify(e => e.GetFinalState(), Times.Once());

            mockDeviceStateAction.Verify(e => e.DoWork(), Times.Once());
        }

        [Fact]
        public void Complete_ShouldMoveToTheNextState_When_Called()
        {
            Mock<IDeviceStateAction> newState = new Mock<IDeviceStateAction>();
            mockDeviceStateActionController.Setup(e => e.GetNextAction(mockDeviceStateAction.Object)).Returns(newState.Object);

            subject.Complete(mockDeviceStateAction.Object).Wait();

            mockDeviceStateActionController.Verify(e => e.GetNextAction(mockDeviceStateAction.Object), Times.Once());

            mockDeviceStateAction.Verify(e => e.Dispose(), Times.Once());

            newState.Verify(e => e.DoWork(), Times.Once());
        }

        [Fact]
        public void Error_ShouldStopWorkflow_When_WorkflowStateIsNone()
        {
            mockDeviceStateAction.Setup(e => e.WorkflowStateType).Returns(DeviceWorkflowState.None);

            subject.Error(mockDeviceStateAction.Object).Wait();

            mockDeviceStateActionController.Verify(e => e.GetFinalState(), Times.Once());
        }

        [Fact]
        public void Error_ShouldContinueToNextState_When_Called()
        {
            Mock<IDeviceStateAction> newState = new Mock<IDeviceStateAction>();
            mockDeviceStateActionController.Setup(e => e.GetNextAction(mockDeviceStateAction.Object)).Returns(newState.Object);

            mockDeviceStateAction.Setup(e => e.WorkflowStateType).Returns(DeviceWorkflowState.DeviceRecovery);

            subject.Error(mockDeviceStateAction.Object).Wait();

            mockDeviceStateActionController.Verify(e => e.GetFinalState(), Times.Never());
        }

        [Fact]
        public void Recovery_ShouldContinueToNextState_When_Called()
        {
            Mock<IDeviceStateAction> newState = new Mock<IDeviceStateAction>();
            mockDeviceStateActionController.Setup(e => e.GetNextAction(mockDeviceStateAction.Object)).Returns(newState.Object);

            subject.Recovery(mockDeviceStateAction.Object, newState).Wait();

            mockDeviceStateActionController.Verify(e => e.GetNextAction(mockDeviceStateAction.Object), Times.Once());
            mockDeviceStateActionController.Verify(e => e.GetFinalState(), Times.Never());

            var stack = TestHelper.Helper.GetFieldValueFromInstance<Stack<object>>("savedStackState", false, false, subject);

            Assert.Empty(stack);
        }

        [Fact]
        public void ChannelClient_ChannelConnected_ShouldSubscribeToTheListenerOnlyOnce_When_ConnectEventTriggeredMoreThanOnce()
        {
            Guid guid = new Guid();

            string expectedValue = guid.ToString().Substring(0, 8);

            TestHelper.Helper.SetPropertyValueToInstance<bool>("subscribed", false, true, subject, false);

            //subject.ChannelClient_ChannelConnected(guid);
            //subject.ChannelClient_ChannelConnected(guid);

            //bool isSubscribed = TestHelper.Helper.GetPropertyValueFromInstance<bool>("subscribed", false, true, subject);

            //Assert.True(isSubscribed);

            //mockListenerConnector.Verify(e => e.Subscribe(It.IsAny<TopicOption>()), Times.Once);
            //mockLoggingServiceClient.Verify(e => e.LogInfoAsync(It.IsRegex(expectedValue), It.IsAny<IDictionary<string, object>>()), Times.AtLeastOnce());
        }

        [Fact]
        public void ChannelClient_ChannelDisconnected_ShouldLogTheEvent_When_Called()
        {
            Guid guid = new Guid();

            string expectedValue = guid.ToString().Substring(0, 8);

            //subject.ChannelClient_ChannelDisconnected(guid);

            bool isSubscribed = TestHelper.Helper.GetPropertyValueFromInstance<bool>("subscribed", false, true, subject);

            //Assert.False(isSubscribed);

            //mockLoggingServiceClient.Verify(e => e.LogInfoAsync(It.IsRegex(expectedValue), It.IsAny<IDictionary<string, object>>()), Times.Once());
        }
    }
}
