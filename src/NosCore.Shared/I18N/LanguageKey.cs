﻿using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NosCore.Shared.I18N
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum LanguageKey
    {
        UNREGISTRED_FROM_MASTER,
        REGISTRED_ON_MASTER,
        UNRECOGNIZED_MASTER_PACKET,
        AUTHENTICATED_SUCCESS,
        AUTHENTICATED_ERROR,
        DATABASE_INITIALIZED,
        DATABASE_NOT_UPTODATE,
        CLIENT_DISCONNECTED,
        CHARACTER_NOT_INIT,
        ERROR_CHANGE_MAP,
        AUTH_INCORRECT,
        AUTH_ERROR,
        FORCED_DISCONNECTION,
        CLIENT_CONNECTED,
        REGISTRED_FROM_MASTER,
        CLIENT_ARRIVED,
        CORRUPTED_KEEPALIVE,
        INVALID_PASSWORD,
        INVALID_ACCOUNT,
        ACCOUNT_ARRIVED,
        SUCCESSFULLY_LOADED,
        MASTER_SERVER_RETRY,
        LISTENING_PORT,
        MASTER_SERVER_LISTENING,
        ENTER_PATH,
        PARSE_ALL,
        PARSE_MAPS,
        PARSE_MAPTYPES,
        PARSE_ACCOUNTS,
        PARSE_PORTALS,
        PARSE_TIMESPACES,
        PARSE_ITEMS,
        PARSE_NPCMONSTERS,
        PARSE_NPCMONSTERDATA,
        PARSE_CARDS,
        PARSE_SKILLS,
        PARSE_MAPNPCS,
        PARSE_MONSTERS,
        PARSE_SHOPS,
        PARSE_TELEPORTERS,
        PARSE_SHOPITEMS,
        PARSE_SHOPSKILLS,
        PARSE_RECIPES,
        PARSE_QUESTS,
        DONE,
        AT_LEAST_ONE_FILE_MISSING,
        CARDS_PARSED,
        ITEMS_PARSED,
        MAPS_PARSED,
        PORTALS_PARSED,
        MAPS_LOADED,
        NO_MAP,
        MAPMONSTERS_LOADED,
        CORRUPT_PACKET,
        HANDLER_ERROR,
        HANDLER_NOT_FOUND,
        SELECT_MAPID,
        WRONG_SELECTED_MAPID,
        I18N_ACTDESC_PARSED,
        I18N_CARD_PARSED,
        I18N_BCARD_PARSED,
        I18N_ITEM_PARSED,
        I18N_MAPIDDATA_PARSED,
        I18N_MAPPOINTDATA_PARSED,
        I18N_MPCMONSTER_PARSED,
        I18N_NPCMONSTERTALK_PARSED,
        I18N_QUEST_PARSED,
        I18N_SKILL_PARSED,
        PARSE_I18N,
        ALREADY_TAKEN,
        INVALID_CHARNAME,
        BAD_PASSWORD,
        SUPPORT,
        [UsedImplicitly] ADVENTURER,
        [UsedImplicitly] SWORDMAN,
        [UsedImplicitly] ARCHER,
        [UsedImplicitly] MAGICIAN,
        [UsedImplicitly] WRESTLER,
        NPCMONSTERS_PARSED,
        PARSE_DROPS,
        MAPTYPES_PARSED,
        RESPAWNTYPE_PARSED,
        SKILLS_PARSED,
        NPCS_PARSED,
        MONSTERS_PARSED,
        CHANNEL,
        ADMINISTRATOR,
        CHARACTER_OFFLINE,
        SEND_MESSAGE_TO_CHARACTER,
        MAPNPCS_LOADED,
        NO_ITEM,
        NOT_ENOUGH_PLACE,
        ITEM_ACQUIRED,
        ITEMS_LOADED,
        ASK_TO_DELETE,
        SURE_TO_DELETE,
        ITEM_NOT_DROPPABLE_HERE,
        DROP_MAP_FULL,
        BAD_DROP_AMOUNT,
        ITEM_NOT_DROPPABLE,
        FRIENDLIST_FULL,
        ALREADY_FRIEND,
        FRIEND_REQUEST_BLOCKED,
        MAX_GOLD,
        ITEM_ACQUIRED_LOD,
        SP_POINTSADDED,
        FRIEND_ADD,
        FRIEND_ADDED,
        FRIEND_REJECTED,
        CANT_BLOCK_FRIEND,
        NOT_IN_BLACKLIST,
        BLACKLIST_ADDED,
        SAVING_ALL,
        BLACKLIST_BLOCKED,
        FRIEND_DELETED,
        FRIEND_OFFLINE,
        CANT_FIND_CHARACTER,
        GROUP_LEADER_CHANGE,
        GROUP_FULL,
        ALREADY_IN_GROUP,
        GROUP_BLOCKED,
        INVITED_YOU_GROUP,
        INVITED_GROUP_SHARE,
        GROUP_SHARE_INFO,
        JOINED_GROUP,
        GROUP_ADMIN,
        GROUP_REFUSED,
        SHARED_REFUSED,
        ACCEPTED_SHARE,
        GOLD_SET,
        NEW_LEADER,
        LEAVE_GROUP,
        GROUP_LEFT,
        GROUP_CLOSED,
        GROUP_INVITE,
        NPCMONSTERS_LOADED,
        UNABLE_TO_REQUEST_GROUP
    }
}