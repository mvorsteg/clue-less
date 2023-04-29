using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "CharacterSpriteMapping", menuName = "")]
public class CharacterSpriteMapping : ScriptableObject
{
    public CharacterType character;
    public Sprite sprite;
}