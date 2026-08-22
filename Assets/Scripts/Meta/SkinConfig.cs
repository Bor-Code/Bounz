using UnityEngine;

[System.Serializable]
public class SkinData
{
    public string id;
    public string displayName;
    public Color color = Color.white;
    public int price;
}

/// <summary>
/// Tüm mevcut skin'leri (kozmetikleri) tanımladığımız ScriptableObject.
/// Project > Create > Bounz > SkinConfig ile oluşturulur.
/// </summary>
[CreateAssetMenu(fileName = "SkinConfig", menuName = "Bounz/SkinConfig")]
public class SkinConfig : ScriptableObject
{
    [Tooltip("İlk sıradaki skin otomatik olarak varsayılan kabul edilir ve kilitli değildir.")]
    public SkinData[] skins;

    public SkinData GetSkin(string id)
    {
        foreach (var s in skins)
        {
            if (s.id == id) return s;
        }
        return skins.Length > 0 ? skins[0] : null;
    }
}
