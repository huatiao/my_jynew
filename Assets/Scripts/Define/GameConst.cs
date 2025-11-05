
using System.Collections.Generic;

namespace ProjectBase.Define
{
    public enum MpType
    {
        Yin = 0,     //阴性内力
        Yang = 1,    //阳性内力
        Neutral = 2, //中性内力
    }

    public struct PropertyItem
    {
        public int ID;
        public string Name;
        public string PropertyName;
        public int DefaulMax;//初始化的时候的最大值
        public int DefaulMin;//初始化的时候的最小值

        public PropertyItem(int ID, string PropertyName, string Name, int DefaulMax, int DefaulMin)
        {
            this.ID = ID;
            this.Name = Name;
            this.PropertyName = PropertyName;
            this.DefaulMax = DefaulMax;
            this.DefaulMin = DefaulMin;
        }
    }

    public enum PropertyID
    {
        MaxHp_Special = -1, //创角用到 特殊
        MaxMp_Special = -2,  //创角用到 特殊
        Attack_Special = -3, //创角用到 特殊

        MpType = 0,       //内力性质
        Qinggong = 3,     //轻功
        Defence = 4,     //防御
        Heal = 6,     //医疗
        UsePoison = 7,     //使毒
        DePoison = 8,
        Quanzhang = 9,     //拳掌
        Yujian = 10,     //御剑
        Shuadao = 11,     //耍刀
        Anqi = 12,     //暗器
        Hp = 13,     //生命
        Tili = 14,     //体力
        Mp = 15,     //内力
        MaxHp = 16,     //最大生命
        MaxMp = 17,     //最大内力
        Attack = 18,     //攻击力
        AntiPoison = 19,     //抗毒
        Qimen = 20,     //特殊兵器
        Wuxuechangshi = 21,     //武学常识
        Pinde = 22,     //品德
        AttackPoison = 23,     //功夫带毒
        Zuoyouhubo = 24,     //左右互搏
        IQ = 25,     //资质
        Poison = 26,     //中毒
        HpInc = 27,     //生命增长
    }

    public class GameConst
    {
        public readonly static SortedDictionary<PropertyID, PropertyItem> ProItemDic = new()
        {
            [PropertyID.MaxMp_Special] = new PropertyItem((int)PropertyID.MaxMp_Special, "MaxMp", "内力", 40, 30),//创角用到 特殊
            [PropertyID.Attack_Special] = new PropertyItem((int)PropertyID.Attack_Special, "Attack", "武力", 30, 20),//创角用到 特殊
            [PropertyID.MaxHp_Special] = new PropertyItem((int)PropertyID.MaxHp_Special, "MaxHp", "生命", 50, 30),//创角用到 特殊

            [PropertyID.MpType] = new PropertyItem((int)PropertyID.MpType, "MpType", "内力性质", (int)MpType.Yang, (int)MpType.Yin),
            [PropertyID.Qinggong] = new PropertyItem((int)PropertyID.Qinggong, "Qinggong", "轻功", 30, 20),
            [PropertyID.Defence] = new PropertyItem((int)PropertyID.Defence, "Defence", "防御", 30, 20),
            [PropertyID.Heal] = new PropertyItem((int)PropertyID.Heal, "Heal", "医疗", 30, 20),
            [PropertyID.UsePoison] = new PropertyItem((int)PropertyID.UsePoison, "UsePoison", "使毒", 30, 20),
            [PropertyID.DePoison] = new PropertyItem((int)PropertyID.DePoison, "DePoison", "解毒", 30, 20),
            [PropertyID.Quanzhang] = new PropertyItem((int)PropertyID.Quanzhang, "Quanzhang", "拳掌", 30, 20),
            [PropertyID.Yujian] = new PropertyItem((int)PropertyID.Yujian, "Yujian", "剑术", 30, 20),
            [PropertyID.Shuadao] = new PropertyItem((int)PropertyID.Shuadao, "Shuadao", "刀术", 30, 20),
            [PropertyID.Anqi] = new PropertyItem((int)PropertyID.Anqi, "Anqi", "暗器", 30, 20),
            [PropertyID.Hp] = new PropertyItem((int)PropertyID.Hp, "Hp", "生命", 30, 20),
            [PropertyID.Tili] = new PropertyItem((int)PropertyID.Tili, "Tili", "体力", 30, 20),
            [PropertyID.Mp] = new PropertyItem((int)PropertyID.Mp, "Mp", "内力", 40, 30),
            [PropertyID.MaxHp] = new PropertyItem((int)PropertyID.MaxHp, "MaxHp", "最大生命", 50, 30),
            [PropertyID.MaxMp] = new PropertyItem((int)PropertyID.MaxMp, "MaxMp", "最大内力", 40, 30),
            [PropertyID.Attack] = new PropertyItem((int)PropertyID.Attack, "Attack", "攻击力", 30, 20),
            [PropertyID.AntiPoison] = new PropertyItem((int)PropertyID.AntiPoison, "AntiPoison", "抗毒", 30, 20),
            [PropertyID.Qimen] = new PropertyItem((int)PropertyID.Qimen, "Qimen", "特殊", 30, 20),
            [PropertyID.Wuxuechangshi] = new PropertyItem((int)PropertyID.Wuxuechangshi, "Wuxuechangshi", "武学常识", 30, 20),
            [PropertyID.Pinde] = new PropertyItem((int)PropertyID.Pinde, "Pinde", "品德", 30, 20),
            [PropertyID.AttackPoison] = new PropertyItem((int)PropertyID.AttackPoison, "AttackPoison", "功夫带毒", 30, 20),
            [PropertyID.Zuoyouhubo] = new PropertyItem((int)PropertyID.Zuoyouhubo, "Zuoyouhubo", "左右互搏", 30, 20),
            [PropertyID.IQ] = new PropertyItem((int)PropertyID.IQ, "IQ", "资质", 100, 30),
            [PropertyID.Poison] = new PropertyItem((int)PropertyID.Poison, "Poison", "中毒", 30, 20),
            [PropertyID.HpInc] = new PropertyItem((int)PropertyID.HpInc, "HpInc", "生命增长", 7, 3),
        };

    }
}
