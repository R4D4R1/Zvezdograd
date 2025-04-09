   using UnityEngine;
   using UniRx;

    public class RepairableBuilding : ChangeMaterialsBuilding,ISaveableBuilding
    {
        [Header("REPAIRABLE SETTINGS")]
        [SerializeField] private RepairableBuildingConfig repairableConfig;
        public RepairableBuildingConfig RepairableBuildingConfig => repairableConfig;
        
        protected int TurnsToRepair { get; set; }
        public State CurrentState;
        
        public BuildingType Type => repairableConfig.BuildingType;

        public enum State
        {
            Intact,
            Damaged,
            Repairing
        }

        public enum BuildingType
        {
            LivingArea,
            Hospital,
            FoodTrucks,
            Factory,
            CityHall
        }

        public override void Init()
        {
            base.Init();

            CurrentState = repairableConfig.State;

            FindBuildingModels();

            TimeController.OnNextTurnBtnClickBetween
                .Subscribe(_ => TryTurnOnBuilding())
                .AddTo(this);
            TimeController.OnNextTurnBtnClickBetween
                .Subscribe(_ => UpdateAmountOfTurnsNeededToDoSMTH())
                .AddTo(this);

            UpdateBuildingModel();
            UpdateAmountOfTurnsNeededToDoSMTH();
        }

        private void UpdateAmountOfTurnsNeededToDoSMTH()
        {
            if (CurrentState != State.Repairing)
                TurnsToRepair = UpdateAmountOfTurnsNeededToDoSmth(repairableConfig.TurnsToRepairOriginal);
        }

        protected virtual void TryTurnOnBuilding()
        {
            if (CurrentState == State.Repairing)
            {
                TurnsToRepair--;

                if (TurnsToRepair == 0)
                {
                    BuildingIsSelectable = true;
                    RestoreOriginalMaterials();
                    CurrentState = State.Intact;
                    UpdateBuildingModel();
                }
            }
        }

        public void RepairBuilding()
        {
            if (CurrentState == State.Damaged)
            {
                PeopleUnitsController.AssignUnitsToTask(repairableConfig.PeopleToRepair, TurnsToRepair, repairableConfig.TurnsToRestFromRepair);
                ResourceViewModel.ModifyResourceCommand.Execute((ResourceModel.ResourceType.ReadyMaterials, -repairableConfig.BuildingMaterialsToRepair));

                BuildingIsSelectable = false;
                SetGreyMaterials();
                CurrentState = State.Repairing;
            }
        }

        public void BombBuilding()
        {
            if (CurrentState == State.Intact)
            {
                BuildingIsSelectable = true;
                CurrentState = State.Damaged;
                UpdateBuildingModel();
            }
        }

        private void FindBuildingModels()
        {
            IntactBuilding intactComponent = GetComponentInChildren<IntactBuilding>();
            DamagedBuilding damagedComponent = GetComponentInChildren<DamagedBuilding>();

            if (intactComponent)
            {
                _intactBuildingModel = intactComponent.gameObject;
            }
            else
            {
                Debug.LogError("IntactBuilding component not found on any child object.");
            }

            if (damagedComponent)
            {
                _damagedBuildingModel = damagedComponent.gameObject;
            }
            else
            {
                Debug.LogError("DamagedBuilding component not found on any child object.");
            }
        }

        private void UpdateBuildingModel()
        {
            SetModelActive(_intactBuildingModel, CurrentState == State.Intact);
            SetModelActive(_damagedBuildingModel, CurrentState != State.Intact);
        }

        private void SetModelActive(GameObject model, bool isActive)
        {
            if (model)
                model.SetActive(isActive);
        }

        private GameObject _intactBuildingModel;
        private GameObject _damagedBuildingModel;
        
        public new int BuildingId => base.BuildingId;
        
        public virtual BuildingSaveData GetSaveData()
        {
            return new RepairableBuildingSaveData
            {
                buildingId = BuildingId,
                buildingIsSelectable = BuildingIsSelectable,
                turnsToRepair = TurnsToRepair,
                currentState = CurrentState
            };
        }

        public virtual void LoadFromSaveData(BuildingSaveData data)
        {
            var save = data as RepairableBuildingSaveData;
            if (save == null) return;

            BuildingIsSelectable = save.buildingIsSelectable;
            TurnsToRepair = save.turnsToRepair;
            CurrentState = save.currentState;

            UpdateBuildingModel();

            if (BuildingIsSelectable)
                RestoreOriginalMaterials();
            else
                SetGreyMaterials();
        }
    }
