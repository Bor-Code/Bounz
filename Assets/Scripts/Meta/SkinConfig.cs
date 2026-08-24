

using UnityEngine;

[System.Serializable]

public class SkinData

{

    public string id;

    public string displayName;

    public Color color = Color.white;

    public int price;

}

[CreateAssetMenu(fileName = "SkinConfig", menuName = "Bounz/SkinConfig")]

public class SkinConfig : ScriptableObject

{

    [Tooltip("İlk sıradaki skin otomatik olarak varsayılan kabul edilir ve kilitli değildir.")]

    public SkinData[] skins;

    public SkinData GetSkin(string id)

    {

        EnsureDefaults();

        foreach (var s in skins)

        {

            if (s.id == id) return s;

        }

        return skins.Length > 0 ? skins[0] : null;

    }



    public static SkinConfig CreateDefault()

    {

        SkinConfig config = CreateInstance<SkinConfig>();

        config.name = "RuntimeSkinConfig";

        config.EnsureDefaults();

        return config;

    }



    public void EnsureDefaults()

    {

        if (skins != null && skins.Length > 0) return;



        skins = new[]

        {

            new SkinData { id = "default", displayName = "Default", color = Color.white, price = 0 },

            new SkinData { id = "blue", displayName = "Blue", color = new Color(0.2f, 0.6f, 1f), price = 100 },

            new SkinData { id = "gold", displayName = "Gold", color = new Color(1f, 0.78f, 0.2f), price = 250 }

        };

    }

}

