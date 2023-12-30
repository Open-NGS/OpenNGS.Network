using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNGSCommon;

public static class NetworkErrMsg
{
    #region 错误信息

    // 错误信息列表（暂时放到这，将来放到配置表中）
    private static Dictionary<int, string> mErrMsg = new Dictionary<int, string>();

    public static Dictionary<int, string> ErrMsg => mErrMsg;
    /// <summary>
    /// 初始化错误信息数组，只设置需要提示的
    /// </summary>
    public static void Init()
    {
        // TODO
        //mErrMsg[(int)ECSErrno.ECSERRNO_NICKNAME_ERROR] = "NetworkError.NicknameError";
        //mErrMsg[(int)ECSErrno.ECSERRNO_NICKNAME_REPEAT_ERROR] = "NetworkError.NicknameRepeat";
        //mErrMsg[(int)ECSErrno.ECSERRNO_MATCH_SCHEME_ERROR] = "NetworkError.MatchSchemeError";
        //mErrMsg[(int)ECSErrno.ECSERRNO_MATCH_STATUS_ERROR] = "NetworkError.MatchStatusError";
        //mErrMsg[(int)ECSErrno.ECSERRNO_MATCH_TIMEOUT] = "NetworkError.MatchTimeout";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_GOLD_LESS] = "NetworkError.ShopGoldLess";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_DUST_LESS] = "NetworkError.ShopDustLess";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_SYSTEM_ERROR] = "NetworkError.ShopSystemError";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_CARD_CANNOT_MAKE] = "NetworkError.ShopCardCannotMake";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_CARD_HAD] = "NetworkError.ShopCardHad";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_PACK_NOT_EXIST] = "NetworkError.ShopPackNotExist";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_PACK_ENOUGH] = "NetworkError.ShopPackEnough";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_CARD_NOT_EXIST] = "NetworkError.ShopCardNotExist";
        //mErrMsg[(int)ECSErrno.ECSERRNO_TIMEBOX_NO_TIME] = "NetworkError.TimeBoxNoTime";
        //mErrMsg[(int)ECSErrno.ECSERRNO_TIMEBOX_NO_BOX] = "NetworkError.TimeBoxNoBox";
        //mErrMsg[(int)ECSErrno.ECSERRNO_CURRENCY_LESS] = "NetworkError.ShopCurrencyLess";
        //mErrMsg[(int)ECSErrno.ECSERRNO_HAS_RESET] = "NetworkError.HasReset{0}";
        //mErrMsg[(int)ECSErrno.ECSERRNO_TSS_ERROR] = "TSSError.{0}";

        //mErrMsg[(int)ECSErrno.ECSERRNO_NO_MATCH_ERROR] = "Rank.MatchOvertime";

        //mErrMsg[(int)ECSErrno.ECSERRNO_PLAYER_VERSION_NOT_MATCH] = "NetworkError.VersionNotMatch";

        //mErrMsg[(int)ECSErrno.ECSERRNO_OPPO_IS_MYSELF] = "Friend.PlayerIsSelf";
        //mErrMsg[(int)ECSErrno.ECSERRNO_OPPO_NOT_EXIST] = "Friend.PlayerNoExist";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SELF_IN_OPPO_FRIEND_LIST] = "Friend.IsAlreadyFriends";
        //mErrMsg[(int)ECSErrno.ECSERRNO_OPPO_IN_SELF_FRIEND_LIST] = "Friend.IsAlreadyFriends";
        //mErrMsg[(int)ECSErrno.ECSERRNO_OPPO_APPLICANT_LIST_FULL] = "Friend.OppositeApplicantListFull";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SELF_FRIEND_LIST_FULL] = "Friend.SelfFriendListFull";
        //mErrMsg[(int)ECSErrno.ECSERRNO_OPPO_REFUSE_ALL] = "Friend.OppositeRefuseFriend";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SELF_IN_OPPO_APPLICANT_LIST] = "Friend.SelfAlreadyInOppositeApplicantList";
        //mErrMsg[(int)ECSErrno.ECSERRNO_OPPO_IN_SELF_APPLICANT_LIST] = "Friend.OppositeAlreadyInSelfApplicantList";
        //mErrMsg[(int)ECSErrno.ECSERRNO_OPPO_FRIEND_LIST_FULL] = "Friend.OppositeFriendListFull";

        //mErrMsg[(int)ECSErrno.ECSERRNO_PLAYER_IN_OFFLINE] = "Friend.Offline";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PLAYER_IN_FIGHTING] = "Friend.Fighting";

        //mErrMsg[(int)ECSErrno.ECSERRNO_PLAYER_IN_FIGHT_INVITING] = "Friend.InviteFightFailed";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PLAYER_CANCEL_FIGHT_INVITE] = "Friend.CancelInviteFight";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PLAYER_COMBAT_STATUS_ERROR] = "Friend.InivteFightError";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PLAYER_REFUSE_ALL_FIGHT_INVITE] = "Friend.AlwaysRefuse";

        //mErrMsg[(int)ECSErrno.ECSERRNO_TEAM_ROOM_NOT_EXIST] = "Team.RoomNotExist";

        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_FAIL] = "PartyApiFail";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_TIMEOUT] = "PartyApiTimeOut";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_NOT_EXIST] = "Party.NotExist";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_PLAYER_NOT_EXIST] = "Party.PlayerNotExit";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_IS_EXIST] = "Party.IsExist";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_ROOM_FIGHTING] = "Party.RoomFighting";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_ROOM_FULL] = "Party.RoomFull";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_NOT_EXIST_RANDOM] = "Party.RandomNotExist";

        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_RES_FAIL] = "PartyResFail";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_STATUS_FAIL] = "PartyStatusFail";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_LAYER_NOT_ENOUGH] = "PartyPlayerNotEnough";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_PLAYER_NOT_READY] = "PartyPlayerNotReady";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_KICK_SELF_ERROR] = "PartyKickSelfError";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_KICK_RIGHT_ERROR] = "PartyKickRightError";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_JOIN_TIME_ERROR] = "Party.JoinTimeError";
        //mErrMsg[(int)ECSErrno.ECSERRNO_PARTY_SETTLEF_FIGHT_ERROR] = "Party.SettleAlready";
        //mErrMsg[(int)ECSErrno.ECSERRNO_OPPO_IS_NOT_MY_APPLICANT] = "Friend.IsAlreadyFriends";

        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_CARD_NOT_HAD] = "Flash.NoCommon";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_FLASH_CARD_NOT_OPEN] = "Flash.UnOpen";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_FLASH_CARD_HAD] = "Flash.HasOwned";
        //mErrMsg[(int)ECSErrno.ECSERRNO_SHOP_FLASH_CARD_NOT_HAD] = "Flash.NotHad";
        //mErrMsg[(int)ECSErrno.ECSERRNO_CARDCOMMENT_COMMENT_TOO_LONG] = "C-ardComment.TextTooLong";
        //mErrMsg[(int)ECSErrno.ECSERRNO_CHAT_MESSAGE_EVIL] = "Tips.IllegalText";
        //mErrMsg[(int)ECSErrno.ECSERRNO_CHAT_MESSAGE_LEVEL_LIMIT] = "Chat.MessageLevelLimit";

        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_NAME_EXISTED] = "Guild.ECSERRNO_GUILD_NAME_EXISTED";// = 15001;            //公会名已经存在
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_SELF_ALREADY_HAVE_GUILD] = "Guild.ECSERRNO_GUILD_SELF_ALREADY_HAVE_GUILD";// = 15002; //玩家自己已经有公会了
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_TAR_ALREADY_HAVE_GUILD] = "Guild.ECSERRNO_GUILD_TAR_ALREADY_HAVE_GUILD";// = 15003;  //目标玩家已经有公会了
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_IS_ENTER_CD] = "Guild.ECSERRNO_GUILD_IS_ENTER_CD";// = 15004;             //还在进入公会的CD中
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_NOT_ALLOW] = "Guild.ECSERRNO_GUILD_NOT_ALLOW";// = 15021;             //对方拒绝邀请
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_NO_GUILD] = "Guild.ECSERRNO_GUILD_NO_GUILD";// = 15005;                //没有公会
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_NO_POWER] = "Guild.ECSERRNO_GUILD_NO_POWER";// = 15006;                //没有权限
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_RANK_LESS] = "Guild.ECSERRNO_GUILD_RANK_LESS";// = 15007;               //加入段位不够
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_APPLIST_FULL] = "Guild.ECSERRNO_GUILD_APPLIST_FULL";// = 15008;            //申请列表已经满了
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_PROCESS_LOCK] = "Guild.ECSERRNO_GUILD_PROCESS_LOCK"; // 15009;            //公会系统还未解锁
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_MONEY_NOT_ENOUGH] = "Guild.ECSERRNO_GUILD_MONEY_NOT_ENOUGH"; // 15010;            //创建的金币不足
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_NAME_DIRTY] = "Guild.ECSERRNO_GUILD_NAME_DIRTY"; // 15011;            	//名字有屏蔽字
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_ANNOUNCE_DIRTY] = "Guild.ECSERRNO_GUILD_ANNOUNCE_DIRTY"; // 15012;            //宣言有屏蔽字
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_NAME_TOO_LONG] = "Guild.ECSERRNO_GUILD_NAME_TOO_LONG"; // 15013;            //名字过长
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_ANNOUNCE_TOO_LONG] = "Guild.ECSERRNO_GUILD_ANNOUNCE_TOO_LONG"; // 15014;        //宣言过长
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_ERROR_TAG] = "Guild.ECSERRNO_GUILD_ERROR_TAG"; // 15015;               //tag非法
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_CLIENT_ERROR] = "Guild.ECSERRNO_GUILD_CLIENT_ERROR"; // 15016;            //客户端参数错误
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_MEMBER_FULL] = "Guild.ECSERRNO_GUILD_MEMBER_FULL"; // 15017;            //公会玩家已经满了
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_FORBID_ENTER] = "Guild.ECSERRNO_GUILD_FORBID_ENTER"; // 15018;            //公会禁止加入
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_SYSTEM_LOCK] = "Guild.ECSERRNO_GUILD_SYSTEM_LOCK"; // 15020;            //工会未解锁
        //mErrMsg[(int)ECSErrno.ECSERRNO_PAY_PROCESS_ERR] = "Guild.ECSERRNO_PAY_PROCESS_ERR"; // 16001;            //支付错误码
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_INVITE_TARGET_IN_CD] = "Guild.ECSERRNO_GUILD_INVITE_TARGET_IN_CD"; // 15022;            //邀请的目标CD中
        //mErrMsg[(int)ECSErrno.ECSERRNO_GUILD_INVITE_TARGET_RANK_ERROR] = "Guild.ECSERRNO_GUILD_INVITE_TARGET_RANK_ERROR"; // 15023;            //邀请的目标段位不符合要求

        //mErrMsg[(int)ECSErrno.ECSERROR_HERO_NOT_EXIST] = "Hero.NotExit"; //英雄不存在

        //mErrMsg[(int)ECSErrno.ECSERRNO_ACTIVITY_BOX_LOTTERY_BOX_ID_NOT_EXIST] = "Activity.NotExist"; //宝箱不存在
        //mErrMsg[(int)ECSErrno.ECSERRNO_ACTIVITY_BOX_LOTTERY_TIME_LIMIT] = "Activity.TimeLimit"; //未到抽奖时间
        //mErrMsg[(int)ECSErrno.ECSERRNO_ACTIVITY_BOX_LOTTERY_REWARD_LIMIT] = "Activity.RewardLimit"; //抽奖次数已达上限

        //mErrMsg[(int)ECSErrno.ECSERRNO_SLG_PVE_TIMELIMIT] = "Pve.TimeLimit"; //探险时间限制
        //mErrMsg[(int)ECSErrno.ECSERRNO_SLG_PVE_DOG_INVALID] = "Pve.PetInvalid"; //
        //mErrMsg[(int)ECSErrno.ECSERRNO_SLG_PVE_POS] = "Pve.InvalidCoreBuildingPos"; //
        //mErrMsg[(int)ECSErrno.ECSERRNO_SLG_PVE_STATE] = "Pve.Exploring"; //
        //mErrMsg[(int)ECSErrno.ECSERRNO_SLG_PVE_SYSTEM] = "Pve.LogicalError"; //
        //mErrMsg[(int)ECSErrno.ECSERRNO_SLG_PVE_REWARD_INVALID] = "Pve.InvalidReward"; //
        //mErrMsg[(int)ECSErrno.ECSERRNO_SLG_PVE_CD] = "Pve.Cooldown"; //
        //mErrMsg[(int)ECSErrno.ECSERRNO_SLG_AVOID_TIME] = "Order.AvoidTime"; //
        mErrMsg[(int)ResultCode.RESULT_RESOURCE_DIAMOND_NOT_ENOUGH] = "Resource.DiamondNotEnough"; //
    }

    #endregion
}
