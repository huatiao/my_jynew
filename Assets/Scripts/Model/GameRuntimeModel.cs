using VContainer;

namespace ProjectBase.Model
{
    /// <summary>
    /// 游戏运行时存档Model
    /// </summary>
    public class GameRuntimeModel
    {
        [Inject] private CharacterModel _characterModel;
        [Inject] private PlayerModel _playerModel;

        /// <summary>
        /// 初始化新游戏
        /// </summary>
        public void InitNewGame()
        {
            // 初始化主角
            _playerModel.Init();
        }
    }
}