using ProjectBase.Define;
using UnityEngine;
using System;
using System.Collections.Generic;
using ProjectBase.LubanGen;

namespace ProjectBase.Model
{
    [Serializable]
    public class Character
    {
        #region 存档数据定义
        [SerializeField] public int Key; //ID
        [SerializeField] public string Name; //姓名

        [SerializeField] public int Sex; //性别
        [SerializeField] public int Level = 1; //等级
        [SerializeField] public int Exp; //经验

        [SerializeField] public int Attack; //攻击力
        [SerializeField] public int Qinggong; //轻功
        [SerializeField] public int Defence; //防御力
        [SerializeField] public int Heal; //医疗
        [SerializeField] public int UsePoison; //用毒
        [SerializeField] public int DePoison; //解毒
        [SerializeField] public int AntiPoison; //抗毒
        [SerializeField] public int Quanzhang; //拳掌
        [SerializeField] public int Yujian; //御剑
        [SerializeField] public int Shuadao; //耍刀
        [SerializeField] public int Qimen; //特殊兵器
        [SerializeField] public int Anqi; //暗器技巧
        [SerializeField] public int Wuxuechangshi; //武学常识
        [SerializeField] public int Pinde; //品德
        [SerializeField] public int AttackPoison; //攻击带毒
        [SerializeField] public int Zuoyouhubo; //左右互搏
        [SerializeField] public int Shengwang; //声望
        [SerializeField] public int IQ; //资质
        [SerializeField] public int HpInc; //生命增长


        [SerializeField] public int ExpForItem; //修炼点数
        // [SerializeField] public List<SkillInstance> Wugongs = new List<SkillInstance>(); //武功
        // [SerializeField] public List<CsRoleItem> Items = new List<CsRoleItem>(); //道具

        [SerializeField] public int Mp;
        [SerializeField] public int MaxMp;
        [SerializeField] public int MpType; //内力性质
        [SerializeField] public int Hp;
        [SerializeField] public int MaxHp;
        [SerializeField] public int Hurt; //受伤程度
        [SerializeField] public int Poison; //中毒程度
        [SerializeField] public int Tili; //体力
        [SerializeField] public int ExpForMakeItem; //物品修炼点

        [SerializeField] public int Weapon; //武器
        [SerializeField] public int Armor; //防具
        [SerializeField] public int Xiulianwupin = -1; //修炼物品

        [SerializeField] public int CurrentSkill = 0; //当前技能
        #endregion

        public int ExpGot; //战斗中获得的经验
        public int PreviousRoundHp; //上一回合的生命值

        public Character()
        {
        }

        public Character(CharacterConfig config)
        {
            Init(config);
        }

        public void Init(CharacterConfig config)
        {
            Name = config.Name;
            Sex = (int)config.Sexual;
            Level = config.Level;
            Exp = config.Exp;
            Hp = config.MaxHp;
            MaxHp = config.MaxHp;
            Mp = config.MaxMp;
            MaxMp = config.MaxMp;
            Weapon = config.Weapon;
            Armor = config.Armor;
            MpType = config.MpType;
            Attack = config.Attack;
            Qinggong = config.Qinggong;
            Defence = config.Defence;
            Heal = config.Heal;
            UsePoison = config.UsePoison;
            DePoison = config.DePoison;
            AntiPoison = config.AntiPoison;
            Quanzhang = config.Quanzhang;
            Yujian = config.Yujian;
            Shuadao = config.Shuadao;
            Qimen = config.Qimen;
            Anqi = config.Anqi;
            Wuxuechangshi = config.Wuxuechangshi;
            Pinde = config.Pinde;
            AttackPoison = config.AttackPoison;
            Zuoyouhubo = config.Zuoyouhubo;
            IQ = config.IQ;
            HpInc = config.HpInc;

            PreviousRoundHp = Hp;
            // Tili = GameConst.MAX_ROLE_TILI;
        }

        public void SetProperty(PropertyID id, int value)
        {
            switch (id)
            {
                case PropertyID.MaxHp or PropertyID.MaxHp_Special:
                    MaxHp = value;
                    break;
                case PropertyID.MaxMp or PropertyID.MaxMp_Special:
                    MaxMp = value;
                    break;
                case PropertyID.Attack or PropertyID.Attack_Special:
                    Attack = value;
                    break;
                case PropertyID.MpType:
                    MpType = value;
                    break;
                case PropertyID.Qinggong:
                    Qinggong = value;
                    break;
                case PropertyID.Defence:
                    Defence = value;
                    break;
                case PropertyID.Heal:
                    Heal = value;
                    break;
                case PropertyID.UsePoison:
                    UsePoison = value;
                    break;
                case PropertyID.DePoison:
                    DePoison = value;
                    break;
                case PropertyID.Quanzhang:
                    Quanzhang = value;
                    break;
                case PropertyID.Yujian:
                    Yujian = value;
                    break;
                case PropertyID.Shuadao:
                    Shuadao = value;
                    break;
                case PropertyID.Anqi:
                    Anqi = value;
                    break;
                case PropertyID.Qimen:
                    Qimen = value;
                    break;
                case PropertyID.Hp:
                    Hp = value;
                    break;
                case PropertyID.Tili:
                    Tili = value;
                    break;
                case PropertyID.Mp:
                    Mp = value;
                    break;
                case PropertyID.AntiPoison:
                    AntiPoison = value;
                    break;
                case PropertyID.Wuxuechangshi:
                    Wuxuechangshi = value;
                    break;
                case PropertyID.Pinde:
                    Pinde = value;
                    break;
                case PropertyID.AttackPoison:
                    AttackPoison = value;
                    break;
                case PropertyID.Zuoyouhubo:
                    Zuoyouhubo = value;
                    break;
                case PropertyID.IQ:
                    IQ = value;
                    break;
                case PropertyID.HpInc:
                    HpInc = value;
                    break;
                case PropertyID.Poison:
                    Poison = value;
                    break;
            }
        }
    }
}