using System;
using System.Collections;
using System.Collections.Generic;
using NSGameConfig;
using UnityEngine;
using Random = System.Random;

namespace NSBuilding
{
    public class BuildingManager
    {
        

        public bool Init()
        {
            return true;
        }

        public void CleanUp()
        {
            _RecoverBuilding.Clear();
            _DiscoverBuilding.Clear();
            _UnInvestigatedBuilding.Clear();
            _BuildingInDark.Clear();
        }

        //已收复区域
        private Dictionary<BuildingRect,BuildingBasic> _RecoverBuilding = new Dictionary<BuildingRect, BuildingBasic>();
        //已侦查区域
        private Dictionary<BuildingRect, BuildingBasic> _DiscoverBuilding = new Dictionary<BuildingRect, BuildingBasic>();
        //已显示区域
        private Dictionary<BuildingRect, BuildingBasic> _UnInvestigatedBuilding = new Dictionary<BuildingRect, BuildingBasic>();
        //未显示区域
        private Dictionary<BuildingRect, BuildingBasic> _BuildingInDark = new Dictionary<BuildingRect, BuildingBasic>();

        /// <summary>
        /// 通过建筑分类获得建筑类型
        /// </summary>
        /// <param name="name">建筑分类</param>
        /// <returns></returns>
        public int[] GetBuildingTypes(string name)
        {
            if (name == "farm")
            {
                return new []{ BuildingType.kFarmLit, BuildingType.kFarmLarge };
            }
            else if (name == "houses")
            {
                return new[] {BuildingType.kVilla, BuildingType.kOvernightParkingLot, BuildingType.kApartment, BuildingType.kMotel};
            }
            else if (name == "defense")
            {
                return new[] { BuildingType.kPoliceOffice, BuildingType.kShoppingMall, BuildingType.kOlma};
            }
            else if (name == "morale")
            {
                return new[] { BuildingType.kChurch, BuildingType.kPub};
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获取防御值
        /// </summary>
        /// <returns></returns>
        public int GetDefenseValue()
        {
            return 0;
        }

        /// <summary>
        /// 点亮建筑，做管理标记
        /// </summary>
        /// <param name="pos"></param>
        public void LightBuilding(BuildingRect pos)
        {
            if (_BuildingInDark.ContainsKey(pos))
            {
                _UnInvestigatedBuilding.Add(pos,_BuildingInDark[pos]);
                _BuildingInDark.Remove(pos);
            }
        }

        /// <summary>
        /// 建筑收复，做管理标记
        /// </summary>
        /// <param name="pos"></param>
        public void RecoverBuilding(BuildingRect pos)
        {
            if (_DiscoverBuilding.ContainsKey(pos))
            {
                _RecoverBuilding.Add(pos,_DiscoverBuilding[pos]);
            }
            else if (_UnInvestigatedBuilding.ContainsKey(pos))
            {
                _RecoverBuilding.Add(pos,_UnInvestigatedBuilding[pos]);
            }
        }

        public BuildingBasic RequestBuilding(int type, BuildingRect pos)
        {
            if (_UnInvestigatedBuilding.ContainsKey(pos))
            {
                return _UnInvestigatedBuilding[pos];
            }
            else if (_DiscoverBuilding.ContainsKey(pos))
            {
                return _DiscoverBuilding[pos];
            }
            else if (_RecoverBuilding.ContainsKey(pos))
            {
                return _RecoverBuilding[pos];
            }
            else if (_BuildingInDark.ContainsKey(pos))
            {
                return _BuildingInDark[pos];
            }
            else
            {
                BuildingBasic building = CreateBuilding(type);
                if (building != null)
                {
                    var temp = ConfigManager.Instance.GetBuildTemplateByType(type);
                    if (temp != null)
                    {
                        building.UpdateFoodNum(Convert.ToInt16(Utils.PickRandom(temp.Foods)));
                        building.UpdateSurvivorsNum(Convert.ToInt16(Utils.PickRandom(temp.Survivor)));
                        building.UpdateEquipNum(Convert.ToInt16(Utils.PickRandom(temp.Equip)));
                        building.PushOptions(temp.Options);
                        building.Name = temp.Name;
                        building.Investigated = false;
                    }

                    _BuildingInDark.Add(pos, building);
                }
                return building;
            }
        }


        private BuildingBasic CreateBuilding(int type)
        {
            BuildingBasic building = null;
            switch (type)
            {
                case BuildingType.kFarmLit:
                    building = new FarmLite();
                    break;
                case BuildingType.kFarmLarge:
                    building = new FarmLarge();
                    break;
                case BuildingType.kVilla:
                    building = new Villa();
                    break;
                case BuildingType.kPark:
                    building = new Park();
                    break;
                case BuildingType.kWasteland:
                    building = new Wasteland();
                    break;
                case BuildingType.kParkingLot:
                    building = new ParkingLot();
                    break;
                case BuildingType.kOvernightParkingLot:
                    building = new OvernightParkingLot();
                    break;
                case BuildingType.kServiceStation:
                    building = new ServiceStation();
                    break;
                case BuildingType.kRuins:
                    building = new Ruins();
                    break;
                case BuildingType.kLaboratory:
                    building = new Laboratory();
                    break;
                case BuildingType.kHispital:
                    building = new Hispital();
                    break;
                case BuildingType.kApartment:
                    building = new Apartment();
                    break;
                case BuildingType.kPub:
                    building = new Pub();
                    break;
                case BuildingType.kChurch:
                    building = new Church();
                    break;
                case BuildingType.kWarehouse:
                    building = new Warehouse();
                    break;
                case BuildingType.kMotel:
                    building = new Motel();
                    break;
                case BuildingType.kConvenienceStore:
                    building = new ConvenienceStore();
                    break;
                case BuildingType.kMcDonald:
                    building = new McDonald();
                    break;
                case BuildingType.kOffice:
                    building = new Office();
                    break;
                case BuildingType.kFoodStore:
                    building = new FoodStore();
                    break;
                case BuildingType.kOlma:
                    building = new Olma();
                    break;
                case BuildingType.kShoppingMall:
                    building = new ShoppingMall();
                    break;
                case BuildingType.kCemetery:
                    building = new Cemetery();
                    break;
                case BuildingType.kCemeteryLite:
                    building = new CemeteryLite();
                    break;
                case BuildingType.kPoliceOffice:
                    building = new PoliceOffice();
                    break;
                case BuildingType.kHelicopterApron:
                    building = new HelicopterApron();
                    break;
                case BuildingType.kCityHall:
                    building = new CityHall();
                    break;
                case BuildingType.kMetro:
                    building = new Metro();
                    break;
                default:
                    return null;
            }

            return building;
        }
    }
}