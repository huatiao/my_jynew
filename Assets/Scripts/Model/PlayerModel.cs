using VContainer;

namespace ProjectBase.Model
{
    public class PlayerModel : Character
    {
        [Inject] private ConfigModel _configModel;

        private bool _isInit = false;

        public void Init()
        {
            if (_isInit)
            {
                return;
            }
            
            _isInit = true;

            var config = _configModel.TbCharacterData.DataList[0];
            Init(config);
        }
    }
}