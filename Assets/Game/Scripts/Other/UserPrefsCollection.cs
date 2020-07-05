using UnityEngine;

[CreateAssetMenu(fileName = "New User Preferences", menuName = "UserPreferences")]
public class UserPrefsCollection : ScriptableObject
{
    public Sprite goldenBallSprite;
    public Sprite negativeEffectHighlight;
    public Sprite positiveEffectSprite;

    public SpriteAndColor[] styleSprites;

    public Sprite[] effectSprites;
    public Material backgroundMaterial;
}