using UnityEngine;
using ProjectBase.UI;
using System.Collections.Generic;
using ProjectBase.Service;
using ProjectBase.Define;
using VContainer;

namespace ProjectBase.Model
{
    public class CharacterModel
    {
        [Inject] private ConfigModel _configModel;

        private List<Character> _allCharacters = new List<Character>();

        public List<Character> AllCharacters => _allCharacters;

        public void Init()
        {
            _allCharacters.Clear();

            foreach (var config in _configModel.TbCharacterData.DataList)
            {
                _allCharacters.Add(new Character(config));
            }
        }
    }
}