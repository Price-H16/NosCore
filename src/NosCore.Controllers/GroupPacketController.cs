﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using NosCore.Configuration;
using NosCore.GameObject;
using NosCore.GameObject.ComponentEntities.Extensions;
using NosCore.GameObject.Networking;
using NosCore.Packets.ClientPackets;
using NosCore.Packets.ServerPackets;
using NosCore.Shared.Enumerations;
using NosCore.Shared.Enumerations.Character;
using NosCore.Shared.Enumerations.Group;
using NosCore.Shared.Enumerations.Interaction;
using NosCore.Shared.I18N;

namespace NosCore.Controllers
{
    public class GroupPacketController : PacketController
    {
        private readonly WorldConfiguration _worldConfiguration;

        [UsedImplicitly]
        public GroupPacketController()
        {

        }

        public GroupPacketController(WorldConfiguration worldConfiguration)
        {
            _worldConfiguration = worldConfiguration;
        }

        /// <summary>
        ///     pjoin packet
        /// </summary>
        /// <param name="pjoinPacket"></param>
        public void ManageGroup(PjoinPacket pjoinPacket)
        {
            var targetSession = ServerManager.Instance.Sessions.Values.FirstOrDefault(s => s.Character.CharacterId == pjoinPacket.CharacterId);

            if (targetSession == null && pjoinPacket.RequestType != GroupRequestType.Sharing)
            {
                return;
            }

            switch (pjoinPacket.RequestType)
            {

                case GroupRequestType.Requested:
                case GroupRequestType.Invited:
                    if (pjoinPacket.CharacterId == 0 || targetSession == null || pjoinPacket.CharacterId == Session.Character.CharacterId)
                    {
                        return;
                    }

                    if (targetSession.Character.Group != null && targetSession.Character.Group.IsGroupFull)
                    {
                        Session.SendPacket(new InfoPacket
                        {
                            Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_FULL, Session.Account.Language)
                        });
                        return;
                    }

                    if (targetSession.Character.Group != null && Session.Character.Group != null)
                    {
                        Session.SendPacket(new InfoPacket
                        {
                            Message = Language.Instance.GetMessageFromKey(LanguageKey.ALREADY_IN_GROUP, Session.Account.Language)
                        });
                        return;
                    }

                    if (Session.Character.IsRelatedToCharacter(pjoinPacket.CharacterId, CharacterRelationType.Blocked))
                    {
                        Session.SendPacket(new InfoPacket
                        {
                            Message = Language.Instance.GetMessageFromKey(LanguageKey.BLACKLIST_BLOCKED, Session.Account.Language)
                        });
                        return;
                    }

                    if (targetSession.Character.GroupRequestBlocked)
                    {
                        Session.SendPacket(new MsgPacket
                        {
                            Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_BLOCKED, Session.Account.Language)
                        });
                        return;
                    }

                    Session.Character.GroupRequestCharacterIds.Add(pjoinPacket.CharacterId);

                    if (Session.Character.Group == null || Session.Character.Group.Type == GroupType.Group)
                    {
                        if (targetSession.Character?.Group == null || targetSession.Character?.Group.Type == GroupType.Group)
                        {
                            Session.SendPacket(new InfoPacket { Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_INVITE, Session.Account.Language) });
                            targetSession.SendPacket(new DlgPacket
                            {
                                Question = Language.Instance.GetMessageFromKey(LanguageKey.INVITED_YOU_GROUP, targetSession.Account.Language),
                                YesPacket = new PjoinPacket { CharacterId = Session.Character.CharacterId, RequestType = GroupRequestType.Accepted },
                                NoPacket = new PjoinPacket { CharacterId = Session.Character.CharacterId, RequestType = GroupRequestType.Declined }
                            });
                        }
                    }

                    break;
                case GroupRequestType.Sharing:

                    if (Session.Character.Group == null)
                    {
                        return;
                    }

                    Session.SendPacket(new InfoPacket
                    {
                        Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_SHARE_INFO, Session.Account.Language)
                    });

                    Session.Character.Group.Characters.Values.Where(s => s.Character.CharacterId != Session.Character.CharacterId).ToList().ForEach(s =>
                    {
                        s.Character.GroupRequestCharacterIds.Add(s.Character.CharacterId);
                        s.SendPacket(new DlgPacket
                        {
                            Question = Language.Instance.GetMessageFromKey(LanguageKey.INVITED_GROUP_SHARE, Session.Account.Language),
                            YesPacket = new PjoinPacket { CharacterId = Session.Character.CharacterId, RequestType = GroupRequestType.AcceptedShare },
                            NoPacket = new PjoinPacket { CharacterId = Session.Character.CharacterId, RequestType = GroupRequestType.DeclinedShare }
                        });
                    });

                    
                    break;
                case GroupRequestType.Accepted:
                    if (targetSession == null)
                    {
                        return;
                    }

                    if (!targetSession.Character.GroupRequestCharacterIds.Contains(Session.Character.CharacterId))
                    {
                        return;
                    }

                    targetSession.Character.GroupRequestCharacterIds.Remove(Session.Character.CharacterId);

                    if (Session.Character.Group != null && targetSession.Character.Group != null)
                    {
                        return;
                    }

                    if (Session.Character.Group != null && Session.Character.Group.IsGroupFull ||
                        targetSession.Character.Group != null && targetSession.Character.Group.IsGroupFull)
                    {
                        Session.SendPacket(new InfoPacket
                        {
                            Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_FULL, Session.Account.Language)
                        });

                        targetSession.SendPacket(new InfoPacket
                        {
                          Message  = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_FULL, targetSession.Account.Language)
                        });
                        return;
                    }

                    if (Session.Character.Group != null)
                    {
                        Session.Character.Group.JoinGroup(targetSession);
                        targetSession.SendPacket(new InfoPacket
                        {
                            Message = Language.Instance.GetMessageFromKey(LanguageKey.JOINED_GROUP, targetSession.Account.Language)
                        });
                    }
                    else if (targetSession.Character.Group != null)
                    {
                        if (targetSession.Character.Group.Type == GroupType.Group)
                        {
                            targetSession.Character.Group.JoinGroup(Session);
                        }
                    }
                    else
                    {
                        Session.Character.Group = new Group(GroupType.Group);
                        Session.Character.Group.JoinGroup(targetSession);
                        Session.Character.Group.JoinGroup(Session);
                        Session.SendPacket(new InfoPacket
                        {
                            Message = Language.Instance.GetMessageFromKey(LanguageKey.JOINED_GROUP, Session.Account.Language)
                        });

                        targetSession.SendPacket(new InfoPacket
                        {
                            Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_ADMIN, targetSession.Account.Language)
                        });

                        targetSession.Character.Group = Session.Character.Group;
                        Session.Character.GroupRequestCharacterIds.Clear();
                        targetSession.Character.GroupRequestCharacterIds.Clear();
                    }

                    if (Session.Character.Group?.Type != GroupType.Group)
                    {
                        return;
                    }

                    if (Session.Character.Group == null)
                    {
                        return;
                    }

                    var currentGroup = Session.Character.Group;

                    foreach (var member in currentGroup.Characters.Values)
                    {
                        member.SendPacket(member.Character.GeneratePinit());
                    }

                    //ServerManager.Instance.Groups[currentGroup.GroupId] = currentGroup;
                    Session.Character.MapInstance?.Broadcast(Session.Character.GeneratePidx());
                    break;
                case GroupRequestType.Declined:
                    if (targetSession == null || !targetSession.Character.GroupRequestCharacterIds.Contains(Session.Character.CharacterId))
                    {
                        return;
                    }

                    targetSession.Character.GroupRequestCharacterIds.Remove(Session.Character.CharacterId);
                    targetSession.SendPacket(new InfoPacket
                    {
                        Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_REFUSED, targetSession.Account.Language)
                    });
                    break;
                case GroupRequestType.AcceptedShare:
                    if (targetSession == null || !targetSession.Character.GroupRequestCharacterIds.Contains(Session.Character.CharacterId))
                    {
                        return;
                    }

                    if (Session.Character.Group == null)
                    {
                        return;
                    }

                    targetSession.Character.GroupRequestCharacterIds.Remove(Session.Character.CharacterId);
                    Session.SendPacket(new MsgPacket
                    {
                        Message = Language.Instance.GetMessageFromKey(LanguageKey.ACCEPTED_SHARE, Session.Account.Language),
                        Type = MessageType.Whisper
                    });

                    //TODO: add a way to change respawn points when system will be done
                    break;
                case GroupRequestType.DeclinedShare:
                    if (targetSession == null || !targetSession.Character.GroupRequestCharacterIds.Contains(Session.Character.CharacterId))
                    {
                        return;
                    }

                    targetSession.Character.GroupRequestCharacterIds.Remove(Session.Character.CharacterId);
                    targetSession.SendPacket(new InfoPacket
                    {
                        Message = Language.Instance.GetMessageFromKey(LanguageKey.SHARED_REFUSED, targetSession.Account.Language)
                    });
                    break;
            }
        }

        public void LeaveGroup(PleavePacket pleavePacket)
        {
            var group = Session.Character.Group;

            if (group == null)
            {
                return;
            }

            if (group.Characters.Count > 2)
            {
                if (group.IsGroupLeader(Session))
                {
                    group.LeaveGroup(Session);
                    ServerManager.Instance.Broadcast(Session, new InfoPacket { Message = Language.Instance.GetMessageFromKey(LanguageKey.NEW_LEADER, Session.Account.Language) }, ReceiverType.OnlySomeone, string.Empty, group.Characters.Values.First().Character.CharacterId);
                }

                if (group.Type != GroupType.Group)
                {
                    return;
                }

                foreach (var member in group.Characters.Values)
                {
                    member.SendPacket(member.Character.GeneratePinit());
                    member.SendPacket(new MsgPacket
                    {
                        Message = string.Format(Language.Instance.GetMessageFromKey(LanguageKey.LEAVE_GROUP, Session.Account.Language), Session.Character.Name)
                    });
                }

                Session.SendPacket(Session.Character.GeneratePinit());
                ServerManager.Instance.Broadcast(Session.Character.GeneratePidx(true));
                Session.SendPacket(new MsgPacket { Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_LEFT, Session.Account.Language) });
            }
            else
            {
                var memberList = new List<ClientSession>();
                memberList.AddRange(group.Characters.Values);

                foreach (var member in memberList)
                {
                    member.SendPacket(new MsgPacket { Message = Language.Instance.GetMessageFromKey(LanguageKey.GROUP_CLOSED, member.Account.Language), Type = MessageType.Whisper });
                    ServerManager.Instance.Broadcast(member.Character.GeneratePidx(true));
                    group.LeaveGroup(member);
                    member.SendPacket(member.Character.GeneratePinit());
                }

                //ServerManager.Instance.Groups.TryRemove(group.GroupId, out _);
            }
        }

        public void GroupTalk(GroupTalkPacket groupTalkPacket)
        {
            if (string.IsNullOrEmpty(groupTalkPacket.Message) || Session.Character.Group == null)
            {
                return;
            }

            ServerManager.Instance.Broadcast(Session, Session.Character.GenerateSpk(new SpeakPacket { Message = groupTalkPacket.Message, SpeakType = SpeakType.Group }), ReceiverType.Group);
        }
    }
}
