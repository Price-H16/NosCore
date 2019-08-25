﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using ChickenAPI.Packets.Enumerations;
using ChickenAPI.Packets.Interfaces;
using JetBrains.Annotations;
using NosCore.Configuration;
using NosCore.Core;
using NosCore.Core.HttpClients;
using NosCore.Core.HttpClients.ChannelHttpClient;
using NosCore.Core.I18N;
using NosCore.Data;
using NosCore.Data.Enumerations.I18N;
using NosCore.Data.WebApi;
using NosCore.GameObject.ComponentEntities.Extensions;
using NosCore.GameObject.ComponentEntities.Interfaces;

namespace NosCore.GameObject.HttpClients.FriendHttpClient
{
    public class MailHttpClient : MasterServerHttpClient, IMailHttpClient
    {
        private readonly ISerializer _serializer;

        public MailHttpClient(IHttpClientFactory httpClientFactory, Channel channel, IChannelHttpClient channelHttpClient,
            List<ItemDto> items, WorldConfiguration worldConfiguration, ISerializer serializer)
            : base(httpClientFactory, channel, channelHttpClient)
        {
            ApiUrl = "api/mail";
            RequireConnection = true;
            _serializer = serializer;
        }

        private MailRequest GenerateMailRequest(ICharacterEntity characterEntity, long receiverId, [CanBeNull] IItemInstanceDto itemInstance,
            short? vnum, short? amount, sbyte? rare,
            byte? upgrade, bool isNosmall)
        {
            var mail = new MailDto
            {
                IsOpened = false,
                Date = DateTime.Now,
                ReceiverId = receiverId,
                IsSenderCopy = false,
                ItemInstanceId = itemInstance?.Id ?? Guid.Empty,
                Title = isNosmall ? "NOSMALL" : characterEntity.Name,
                SenderId = isNosmall ? (long?)null : characterEntity.VisualId,
                SenderCharacterClass = isNosmall ? (CharacterClassType?)null : characterEntity.Class,
                SenderGender = isNosmall ? (GenderType?)null : characterEntity.Gender,
                SenderHairColor = isNosmall ? (HairColorType?)null : characterEntity.HairColor,
                SenderHairStyle = isNosmall ? (HairStyleType?)null : characterEntity.HairStyle,
                Hat = isNosmall ? null : characterEntity.Equipment.Hat,
                Armor = isNosmall ? null : characterEntity.Equipment.Armor,
                MainWeapon = isNosmall ? null : characterEntity.Equipment.MainWeapon,
                SecondaryWeapon = isNosmall ? null : characterEntity.Equipment.SecondaryWeapon,
                Mask = isNosmall ? null : characterEntity.Equipment.Mask,
                Fairy = isNosmall ? null : characterEntity.Equipment.Fairy,
                CostumeSuit = isNosmall ? null : characterEntity.Equipment.CostumeSuit,
                CostumeHat = isNosmall ? null : characterEntity.Equipment.CostumeHat,
                WeaponSkin = isNosmall ? null : characterEntity.Equipment.WeaponSkin,
                WingSkin = isNosmall ? null : characterEntity.Equipment.WingSkin,
                SenderMorphId = isNosmall ? (short?)null : characterEntity.Morph == 0 ? (short)-1 : (short)(characterEntity.Morph > short.MaxValue ? 0 : characterEntity.Morph)
            };
            return new MailRequest { Mail = mail, VNum = vnum, Amount = amount, Rare = rare, Upgrade = upgrade };
        }
        public void SendGift(ICharacterEntity characterEntity, long receiverId, IItemInstanceDto itemInstance, bool isNosmall)
        {
            Post<LanguageKey>(GenerateMailRequest(characterEntity, receiverId, itemInstance, null, null, null, null, isNosmall));

            if (characterEntity.VisualId == receiverId)
            {
                characterEntity.SendPacket(characterEntity.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey(LanguageKey.ITEM_GIFTED, characterEntity.AccountLanguage), itemInstance.Amount), SayColorType.Green));
            }
        }

        public void SendGift(ICharacterEntity characterEntity, long receiverId, short vnum, short amount, sbyte rare,
            byte upgrade, bool isNosmall)
        {
            Post(GenerateMailRequest(characterEntity, receiverId, null, vnum, amount, rare, upgrade, isNosmall)).Wait();

            if (characterEntity.VisualId == receiverId)
            {
                characterEntity.SendPacket(characterEntity.GenerateSay(
                    string.Format(Language.Instance.GetMessageFromKey(LanguageKey.ITEM_GIFTED, characterEntity.AccountLanguage), amount), SayColorType.Green));
            }
        }
    }
}
