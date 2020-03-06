using System.Collections.Generic;

namespace DEVICE_CORE.Helpers.Tests
{
    internal static class RequestBuilder
    {
        /*internal static LinkRequest LinkRequestGetDalStatus() =>
            new LinkRequest()
            {
                Actions = new List<LinkActionRequest> { BuildLinkGetDalStatusAction() },
                MessageID = RandomGenerator.BuildRandomString(8),
                Timeout = 1000
            };

        internal static LinkRequest LinkGetCardDataWorkflowRequest() =>
            new LinkRequest()
            {
                Actions = new List<LinkActionRequest> { BuildLinkPaymentRequestAction() },
                MessageID = RandomGenerator.BuildRandomString(8),
                Timeout = 1000
            };

        internal static LinkRequest LinkRequestSelectCreditOrDebit() =>
            new LinkRequest()
            {
                Actions = new List<LinkActionRequest> { BuildLinkGetCreditOrDebit() },
                MessageID = RandomGenerator.BuildRandomIntegerString(1),
                Timeout = 1000
            };

        internal static LinkRequest LinkRequestGetZip() =>
            new LinkRequest()
            {
                Actions = new List<LinkActionRequest> { BuildLinkGetZipAction() },
                MessageID = RandomGenerator.BuildRandomIntegerString(5),
                Timeout = 1000
            };

        internal static LinkRequest LinkRequestGetPin() =>
            new LinkRequest()
            {
                Actions = new List<LinkActionRequest> { BuildLinkGetPinAction() },
                MessageID = RandomGenerator.BuildRandomIntegerString(4),
                Timeout = 1000
            };

        internal static LinkRequest LinkRequestDeviceUI() =>
            new LinkRequest()
            {
                Actions = new List<LinkActionRequest> { BuildLinkDeviceUIAction() },
                MessageID = RandomGenerator.BuildRandomIntegerString(4),
                Timeout = 1000
            };

        internal static LinkRequest LinkDebitWorkflowRequest() =>
            new LinkRequest()
            {
                Actions = new List<LinkActionRequest> { BuildLinkDebitWorkflow() },
                MessageID = RandomGenerator.BuildRandomString(8),
                Timeout = 1000
            };

        private static LinkActionRequest BuildLinkGetDalStatusAction()
        {
            var action = new LinkActionRequest
            {
                MessageID = RandomGenerator.BuildRandomString(8),
                Action = LinkAction.DALAction,
                DALActionRequest = new XO.Requests.DAL.LinkDALActionRequest
                {
                    DALAction = XO.Requests.DAL.LinkDALActionType.GetStatus
                }
            };
            return action;
        }

        private static LinkActionRequest BuildLinkDebitWorkflow()
        {
            var action = new LinkActionRequest
            {
                MessageID = RandomGenerator.BuildRandomString(RandomGenerator.rnd().Next(5, 16)),
                Action = LinkAction.DALAction,
                DALActionRequest = new XO.Requests.DAL.LinkDALActionRequest
                {
                    DALAction = XO.Requests.DAL.LinkDALActionType.GetPayment
                },
                PaymentRequest = new XO.Requests.Payment.LinkPaymentRequest
                {
                    RequestedAmount = 378,
                    CurrencyCode = "USD",
                    PaymentType = XO.Requests.Payment.LinkPaymentRequestType.Sale,
                    PartnerRegistryKeys = new List<string> { "CDTFsL8" },
                    RequestedTenderType = XO.Requests.Payment.LinkPaymentRequestedTenderType.Card,
                    PaymentAttributes = new XO.Requests.Payment.LinkPaymentAttributes
                    {
                        PartialPayment = true
                    },
                    CardWorkflowControls = new XO.Common.DAL.LinkCardWorkflowControls
                    {
                        DebitEnabled = true,
                        AVSEnabled = true,
                        AVSType = new List<string> { "ZIP" }
                    }
                },
                DALRequest = new XO.Requests.DAL.LinkDALRequest
                {
                    LinkObjects = null
                }
            };
            return action;
        }

        private static LinkActionRequest BuildLinkPaymentRequestAction()
        {
            var action = new LinkActionRequest
            {
                Action = LinkAction.DALAction,
                DALActionRequest = new XO.Requests.DAL.LinkDALActionRequest
                {
                    DALAction = XO.Requests.DAL.LinkDALActionType.GetPayment
                },
                PaymentRequest = new XO.Requests.Payment.LinkPaymentRequest
                {
                    RequestedAmount = 378,
                    CurrencyCode = "USD",
                    PaymentType = XO.Requests.Payment.LinkPaymentRequestType.Sale,
                    PartnerRegistryKeys = new List<string> { "CDTFsL8" },
                    RequestedTenderType = XO.Requests.Payment.LinkPaymentRequestedTenderType.Card,
                    PaymentAttributes = new XO.Requests.Payment.LinkPaymentAttributes
                    {
                        PartialPayment = true
                    },
                    CardWorkflowControls = new XO.Common.DAL.LinkCardWorkflowControls
                    {
                        DebitEnabled = false
                    }
                },
                DALRequest = new XO.Requests.DAL.LinkDALRequest
                {
                    LinkObjects = null
                }
            };
            return action;
        }

        private static LinkActionRequest BuildLinkGetCreditOrDebit()
        {
            var action = new LinkActionRequest
            {
                MessageID = RandomGenerator.BuildRandomString(RandomGenerator.rnd().Next(5, 16)),
                Action = LinkAction.DALAction,
                DALActionRequest = new XO.Requests.DAL.LinkDALActionRequest
                {
                    DALAction = XO.Requests.DAL.LinkDALActionType.GetCreditOrDebit
                },
                PaymentRequest = new XO.Requests.Payment.LinkPaymentRequest
                {
                    RequestedAmount = 378,
                    CurrencyCode = "USD",
                    PaymentType = XO.Requests.Payment.LinkPaymentRequestType.Sale,
                    PartnerRegistryKeys = new List<string> { "CDTFsL8" },
                    RequestedTenderType = XO.Requests.Payment.LinkPaymentRequestedTenderType.Card,
                    PaymentAttributes = new XO.Requests.Payment.LinkPaymentAttributes
                    {
                        PartialPayment = true
                    },
                    CardWorkflowControls = new XO.Common.DAL.LinkCardWorkflowControls
                    {
                        DebitEnabled = true,
                        AVSEnabled = true,
                        AVSType = new List<string> { "ZIP" }
                    }
                }
            };
            return action;
        }

        private static LinkActionRequest BuildLinkGetZipAction()
        {
            var action = new LinkActionRequest
            {
                MessageID = RandomGenerator.BuildRandomString(RandomGenerator.rnd().Next(5, 16)),
                Action = LinkAction.DALAction,
                DALActionRequest = new XO.Requests.DAL.LinkDALActionRequest
                {
                    DALAction = XO.Requests.DAL.LinkDALActionType.GetZIP
                },
                PaymentRequest = new XO.Requests.Payment.LinkPaymentRequest
                {
                    RequestedAmount = 378,
                    CurrencyCode = "USD",
                    PaymentType = XO.Requests.Payment.LinkPaymentRequestType.Sale,
                    PartnerRegistryKeys = new List<string> { "CDTFsL8" },
                    RequestedTenderType = XO.Requests.Payment.LinkPaymentRequestedTenderType.Card,
                    PaymentAttributes = new XO.Requests.Payment.LinkPaymentAttributes
                    {
                        PartialPayment = true
                    },
                    CardWorkflowControls = new XO.Common.DAL.LinkCardWorkflowControls
                    {
                        DebitEnabled = false,
                        AVSEnabled = true,
                        AVSType = new List<string> { "ZIP" }
                    }
                },
                DALRequest = new XO.Requests.DAL.LinkDALRequest
                {
                    LinkObjects = null
                }
            };
            return action;
        }

        private static LinkActionRequest BuildLinkGetPinAction()
        {
            var action = new LinkActionRequest
            {
                MessageID = RandomGenerator.BuildRandomString(RandomGenerator.rnd().Next(5, 16)),
                Action = LinkAction.DALAction,
                DALActionRequest = new XO.Requests.DAL.LinkDALActionRequest
                {
                    DALAction = XO.Requests.DAL.LinkDALActionType.GetPIN
                },
                PaymentRequest = new XO.Requests.Payment.LinkPaymentRequest
                {
                    RequestedAmount = 378,
                    CurrencyCode = "USD",
                    PaymentType = XO.Requests.Payment.LinkPaymentRequestType.Sale,
                    PartnerRegistryKeys = new List<string> { "CDTFsL8" },
                    RequestedTenderType = XO.Requests.Payment.LinkPaymentRequestedTenderType.Card,
                    PaymentAttributes = new XO.Requests.Payment.LinkPaymentAttributes
                    {
                        PartialPayment = true
                    },
                    CardWorkflowControls = new XO.Common.DAL.LinkCardWorkflowControls
                    {
                        DebitEnabled = true
                    }
                },
                DALRequest = new XO.Requests.DAL.LinkDALRequest
                {
                    LinkObjects = null
                }
            };
            return action;
        }

        private static LinkActionRequest BuildLinkDeviceUIAction()
        {
            var action = new LinkActionRequest
            {
                MessageID = RandomGenerator.BuildRandomString(RandomGenerator.rnd().Next(5, 16)),
                Action = LinkAction.DALAction,
                DALActionRequest = new XO.Requests.DAL.LinkDALActionRequest
                {
                    DALAction = XO.Requests.DAL.LinkDALActionType.DeviceUI
                },
                PaymentRequest = new XO.Requests.Payment.LinkPaymentRequest
                {
                },
                DALRequest = new XO.Requests.DAL.LinkDALRequest
                {
                    LinkObjects = null
                }
            };
            return action;
        }
        */
    }
}
