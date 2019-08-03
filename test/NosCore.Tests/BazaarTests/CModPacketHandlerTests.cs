﻿using ChickenAPI.Packets.ClientPackets.Bazaar;
using ChickenAPI.Packets.ServerPackets.UI;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NosCore.Core.I18N;
using NosCore.Data;
using NosCore.Data.Enumerations.I18N;
using NosCore.Data.WebApi;
using NosCore.GameObject.HttpClients.BazaarHttpClient;
using NosCore.GameObject.Networking;
using NosCore.GameObject.Networking.ClientSession;
using NosCore.GameObject.Providers.ItemProvider;
using NosCore.PacketHandlers.CharacterScreen;
using NosCore.Tests.Helpers;
using Serilog;
using System;
using System.Linq;

namespace NosCore.Tests.BazaarTests
{
    [TestClass]
    public class CModPacketHandlerTest
    {
        private CModPacketHandler _cmodPacketHandler;
        private ClientSession _session;
        private Mock<IBazaarHttpClient> _bazaarHttpClient;
        private static readonly ILogger _logger = Logger.GetLoggerConfiguration().CreateLogger();

        [TestInitialize]
        public void Setup()
        {
            TestHelpers.Reset();
            Broadcaster.Reset();
            _session = TestHelpers.Instance.GenerateSession();
            _bazaarHttpClient = new Mock<IBazaarHttpClient>();
            _cmodPacketHandler = new CModPacketHandler(_bazaarHttpClient.Object, _logger);

            _bazaarHttpClient.Setup(b => b.GetBazaarLink(0)).Returns(
                new BazaarLink
                {
                    SellerName = "test",
                    BazaarItem = new BazaarItemDto { Price = 50, Amount = 1 },
                    ItemInstance = new ItemInstanceDto { ItemVNum = 1012, Amount = 1 }
                });
            _bazaarHttpClient.Setup(b => b.GetBazaarLink(2)).Returns(
                new BazaarLink
                {
                    SellerName = _session.Character.Name,
                    BazaarItem = new BazaarItemDto { Price = 60, Amount = 1 },
                    ItemInstance = new ItemInstanceDto { ItemVNum = 1012 }
                });
            _bazaarHttpClient.Setup(b => b.GetBazaarLink(1)).Returns((BazaarLink)null);
            _bazaarHttpClient.Setup(b => b.Remove(It.IsAny<long>(), It.IsAny<int>(), It.IsAny<string>())).Returns(true);
        }

        [TestMethod]
        public void ModifyWhenInExchange()
        {
            _session.Character.InExchangeOrTrade = true;
            Assert.IsNull(_session.LastPacket.FirstOrDefault());
        }

        [TestMethod]
        public void ModifyWhenNoItem()
        {
            _cmodPacketHandler.Execute(new CModPacket
            {
                BazaarId = 1,
                NewPrice = 50,
                Amount = 1,
                VNum = 1012,
            }, _session);
            var lastpacket = (ModalPacket)_session.LastPacket.FirstOrDefault(s=>s is ModalPacket);
            Assert.IsTrue(lastpacket.Message == Language.Instance.GetMessageFromKey(LanguageKey.STATE_CHANGED_BAZAAR, _session.Account.Language));
        }


        [TestMethod]
        public void ModifyWhenOtherSeller()
        {
            _cmodPacketHandler.Execute(new CModPacket
            {
                BazaarId = 0,
                NewPrice = 50,
                Amount = 1,
                VNum = 1012,
            }, _session);
            Assert.IsNull(_session.LastPacket.FirstOrDefault());
        }

        [TestMethod]
        public void ModifyWhenSold()
        {
            //clientSession.SendPacket(new ModalPacket
            //                    {
            //                        Message = Language.Instance.GetMessageFromKey(LanguageKey.CAN_NOT_MODIFY_SOLD_ITEMS, clientSession.Account.Language),
            //                        Type = 1
            //                    });
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ModifyWhenWrongAmount()
        {
            //                    clientSession.SendPacket(new ModalPacket
            //                    {
            //                        Message = Language.Instance.GetMessageFromKey(LanguageKey.STATE_CHANGED_BAZAAR, clientSession.Account.Language),
            //                        Type = 1
            //                    });
            throw new NotImplementedException();
        }

        [TestMethod]
        public void ModifyWhenPriceSamePrice()
        {
            //                    clientSession.SendPacket(new ModalPacket
            //                    {
            //                        Message = Language.Instance.GetMessageFromKey(LanguageKey.STATE_CHANGED_BAZAAR, clientSession.Account.Language),
            //                        Type = 1
            //                    });
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Modify()
        {
            //                    clientSession.Character.GenerateSay(
            //                        string.Format(Language.Instance.GetMessageFromKey(LanguageKey.BAZAAR_PRICE_CHANGED, clientSession.Account.Language),
            //                        bz.BazaarItem.Price
            //                    ), SayColorType.Yellow);
            throw new NotImplementedException();
        }
    }
}
