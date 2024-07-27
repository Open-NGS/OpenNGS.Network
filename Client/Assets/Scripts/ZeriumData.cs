using OpenNGS;
using OpenNGS.Exchange.Data;
using OpenNGS.Shop.Data;
using OpenNGS.Systems;
using OpenNGS.UI;
using UnityEngine;

public class ZeriumData : MonoBehaviour
{
    private ICharacterSystem _charSys;
    // Start is called before the first frame update
    void Start()
    {
        GameInstance.Instance.Init();
        ZeriumStaticData.Init();
        UIManager.Init();
        UIManager.Instance.Open("UI_NPC_INTERACTION");

        GameInstance.Instance.InitGameContext(GameContextType.World, new WorldGameContext());

        ExchangeLocalAPI.Instance.Init();
        ExchangeService.Instance.Init(ExchangeLocalAPI.Instance);
        ShopLocalAPI.Instance.Init();
        ShopService.Instance.Init(ShopLocalAPI.Instance);
    }

    private void OnDestroy()
    {
        GameInstance.Instance.Clear();
    }

    [ContextMenu("BuyItem")]
    public void BuyItem()
    {
        BuyReq buyReq = new BuyReq();
        buyReq.ShopId = 1;
        buyReq.ShelfId = 2;
        buyReq.GoodId = 38;
        buyReq.ColIdex = 1;
        buyReq.GoodCounts = 5;
        BuyRsp rep = ShopService.Instance.BugItem(buyReq);
    }

    [ContextMenu("ExchangeItem")]
    public void Exchange()
    {
        ExchangeByGridIDReq request = new ExchangeByGridIDReq();
        GridSrcState src = new GridSrcState();
        src.Col = 1;
        src.Grid = 1;
        src.Counts = 1;

        TargetState trg = new TargetState();
        trg.Col = 1;
        trg.ItemID = 1;
        trg.Counts = 1;
        request.Source.Add(src);
        request.Target.Add(trg);
        ExchangeService.Instance.ExchangeItemByGrid(request);
    }
    [ContextMenu("ClearSaveItem")]
    public void ClearSaveItem()
    {
        //ISaveSystem _isave = App.GetService<ISaveSystem>();
        //SaveFileData_Item data = new SaveFileData_Item();

        //_isave.SetFileData("ITEM", data);
        //_isave.SaveFile();
    }

    [ContextMenu("AddFile")]
    public void AddFile()
    {
        //ISaveSystem _isave = App.GetService<ISaveSystem>();
        //_isave.AddFile();
        //SaveFileData_Item data = new SaveFileData_Item();
        //data._items[1002] = new ItemSaveData();
        //data._items[1002].ItemID = 1002;
        //data._items[1002].Count = 20;

        //_isave.SetFileData("ITEM", data);
        //_isave.SaveFile();
    }

    [ContextMenu("CreateCharacter")]
    public void CreateCharacter()
    {
        _charSys.CreateCharacter("name test");
    }

    [ContextMenu("LoadFile")]
    public void LoadFile()
    {
        //ISaveSystem _isave = App.GetService<ISaveSystem>();

        //_isave.ChangeFile();
    }

    [ContextMenu("LoadCharacter")]
    public void LoadCharacter()
    {
        //OpenNGS.Character.Common.CharacterInfo _chaddr = _charSys.GetCharacterInfo(0);
        _charSys.RefreshCharacter();
        OpenNGS.Character.Common.CharacterInfo _chaddr = _charSys.GetCharacterInfo(0);
    }

    [ContextMenu("AddItemStat")]
    public void AddItem(  )
    {
        IStatSystem _statSys = App.GetService<IStatSystem>();
        if(_statSys != null )
        {
            _statSys.UpdateStat(OpenNGS.Statistic.Common.STAT_EVENT.STAT_EVENT_ADD_ITEM, 1, 3);
        }
    }
    private void Update()
    {
        //Zerium.Data.MonsterData _b = ZeriumStaticData.lstData.GetItem(2, 1);
        //List <Zerium.Data.MonsterData> _b = ZeriumStaticData.lstData.GetItems(2);
        //Zerium.Data.ZeriumData _zd = ZeriumStaticData.lstDataA.GetItem(2, Zerium.Data.MONSTER_TYPE.MONSTER_TYPE_ONE);
        //int a = 0;
        //OpenNGS.NPC.Data.NPCData c = ZeriumStaticData.NPCData.GetItem(1);
        //int b = (int)ZeriumStaticData.NPCData.GetItem(1).UIID[0];
        //int a = 0;
    }
}

public class ZeriumStaticData
{
    //  下面的几个静态变量可以去除。现在作为样例放在这里。
    public static Table<OpenNGS.NPC.Data.NPCData, uint> NPCData = new Table<OpenNGS.NPC.Data.NPCData, uint>((item) => { return item.ID; }, false);
    public static Table<OpenNGS.UI.Data.UIConfig, string> UICfg = new Table<OpenNGS.UI.Data.UIConfig, string>((item) => { return item.IdOfUI; }, false);
    public static Table<Zerium.Data.MonsterData, uint> MonstersData = new Table<Zerium.Data.MonsterData, uint>((item) => { return item.ID; }, false);
    public static Table<Zerium.Data.ZeriumData, uint> ZDatas = new Table<Zerium.Data.ZeriumData, uint>((item) => { return item.ID; }, false);
    public static ListTableBase<Zerium.Data.MonsterData, uint> lstData = new ListTableBase<Zerium.Data.MonsterData, uint>((item) => { return item.ID; }, false);

    public static Table<Zerium.Data.ZeriumData, uint, Zerium.Data.MONSTER_TYPE> lstDataA = 
        new Table<Zerium.Data.ZeriumData, uint, Zerium.Data.MONSTER_TYPE>(
            (item) => { return item.ID; }, (item) => { return item.MonsterTyp; }, false);

    public static SettingTable<Zerium.Data.MonsterData, uint> zd = new SettingTable<Zerium.Data.MonsterData, uint>((item) => { return item.ID; }, false);

    public static void Init()
    {
        NGSStaticData.Init();
        DataManager.Instance.Init();
    }
}
