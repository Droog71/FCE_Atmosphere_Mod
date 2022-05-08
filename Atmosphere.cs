using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Collections;

public class Atmosphere : FortressCraftMod
{
    // Sky sphere, textures and mist/dust objects.
    private GameObject skySphere;
    private GameObject surfaceMistClose;
    private GameObject surfaceDust;
    private GameObject mistCloud;
    private GameObject dustParticles;
    private GameObject snowMist;
    private GameObject toxicMist;
    private GameObject snowDust;
    private GameObject coldMist;
    private Texture2D menuBackgroundTexture;
    private Texture2D lightMenuBackgroundTexture;
    private Texture2D skySphereTexture;
    private Texture2D skySphereTexture_2;
    private Texture2D skySphereTexture_3;
    private Texture2D skySphereNightTexture;
    private Texture2D treeTexture;
    private Texture2D cactusTexture;
    private LocalPlayerScript player;
    private PlayerInventory playerInventory;

    // Settings
    private int selectedSky = 1;
    private int fog_enabled;
    private float fogRed;
    private float fogGreen;
    private float fogBlue;
    private int surface_dust;
    private int surface_mist;
    private int dust_particles;
    private int mist_cloud;
    private int snow_mist;
    private int snow_dust;
    private int cold_mist;
    private int toxic_mist;
    private string terrainTexture = "Normal";
    private float fogDistance = 200;

    // Effects
    private int sepia;
    private int blur_bloom;
    private int mars;
    private int cell_shading;
    private int gray_scale;
    private int eight_bit;
    private int arcade;
    private int blur_focus;
    private int night_vision;
    private int colors_hsv;
    private int chromatic;
    private int old_tv;
    private int hue_rotate;

    // Cheats
    private int disable_fall_damage;
    private int jetpack;
    private int toxic_filter;
    private int ray_gun;
    private int mk2_build_gun;
    private int mk3_build_gun;

    // Menus.
    private bool guiEnabled;
    private bool effectsMenuOpen;
    private bool cheatsMenuOpen;
    private bool settingsMenuOpen;
    private bool confirmBoxOpen;
    private bool fogColorBoxOpen;

    // Player was found in the scene.
    private bool foundPlayer;

    // Player position was found.
    private bool gotPosition;

    // Prevent new glow tube models from using the wrong material.
    private bool lockGlowTubes;

    // Player acknowledged restart required.
    private bool restartConfirmed;

    // Player position.
    private long oX = 0;
    private long oY = 0;
    private long oZ = 0;

    // The main camera.
    private Camera mCam;

    // Mod initialization.
    private bool atmosphereInit;

    // File locations.
    private static readonly string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private static readonly string treeFilePath = Path.Combine(assemblyFolder, "Models/tree.obj");
    private static readonly string cactusFilePath = Path.Combine(assemblyFolder, "Models/cactus.obj");
    private static readonly string settingsFilePath = Path.Combine(assemblyFolder, "settings.txt");
    private static readonly string effectsFilePath = Path.Combine(assemblyFolder, "effects.txt");
    private static readonly string cheatsFilePath = Path.Combine(assemblyFolder, "cheats.txt");
    private static readonly string terrainDataXmlString = Path.Combine(assemblyFolder, "Xml/TerrainData.xml");
    private static readonly string dirtTerrainXmlString = Path.Combine(assemblyFolder, "Xml/Dirt.xml");
    private static readonly string grassTerrainXmlString = Path.Combine(assemblyFolder, "Xml/Grass.xml");
    private static readonly string snowTerrainXmlString = Path.Combine(assemblyFolder, "Xml/Snow.xml");
    private static readonly string menuBackgroundTextureString = Path.Combine(assemblyFolder, "Images/menu.png");
    private static readonly string lightMenuBackgroundTextureString = Path.Combine(assemblyFolder, "Images/fog_menu.png");
    private static readonly string skySphereTextureString = Path.Combine(assemblyFolder, "Images/sky_day.jpg");
    private static readonly string skySphereTextureString_2 = Path.Combine(assemblyFolder, "Images/sky_day_2.jpg");
    private static readonly string skySphereTextureString_3 = Path.Combine(assemblyFolder, "Images/sky_none.png");
    private static readonly string skySphereNightTextureString = Path.Combine(assemblyFolder, "Images/sky_night.jpg");
    private static readonly string treeTextureFilePath = Path.Combine(assemblyFolder, "Images/tree.jpg");
    private static readonly string cactusTextureFilePath = Path.Combine(assemblyFolder, "Images/cactus.jpg");
    private UriBuilder menuTexUriBuildier = new UriBuilder(menuBackgroundTextureString);
    private UriBuilder lightMenuTexUriBuildier = new UriBuilder(lightMenuBackgroundTextureString);
    private UriBuilder dayTexUriBuildier = new UriBuilder(skySphereTextureString);
    private UriBuilder dayTexUriBuildier_2 = new UriBuilder(skySphereTextureString_2);
    private UriBuilder dayTexUriBuildier_3 = new UriBuilder(skySphereTextureString_3);
    private UriBuilder nightTexUriBuilder = new UriBuilder(skySphereNightTextureString);
    private UriBuilder treeTextureUriBuilder = new UriBuilder(treeTextureFilePath);
    private UriBuilder cactusTextureUriBuilder = new UriBuilder(cactusTextureFilePath);

    // Mod registry.
    public override ModRegistrationData Register()
    {
        ModRegistrationData modRegistrationData = new ModRegistrationData();
        return modRegistrationData;
    }

    // Initializes variables.
    public IEnumerator Start()
    {
        menuTexUriBuildier.Scheme = "file";
        dayTexUriBuildier.Scheme = "file";
        dayTexUriBuildier_2.Scheme = "file";
        dayTexUriBuildier_3.Scheme = "file";
        nightTexUriBuilder.Scheme = "file";
        treeTextureUriBuilder.Scheme = "file";
        cactusTextureUriBuilder.Scheme = "file";
        lightMenuTexUriBuildier.Scheme = "file";

        menuBackgroundTexture = new Texture2D(598, 358, TextureFormat.DXT5, false);
        using (WWW www = new WWW(menuTexUriBuildier.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(menuBackgroundTexture);
        }

        lightMenuBackgroundTexture = new Texture2D(598, 358, TextureFormat.DXT5, false);
        using (WWW www = new WWW(lightMenuTexUriBuildier.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(lightMenuBackgroundTexture);
        }

        skySphereTexture = new Texture2D(8000, 2000, TextureFormat.DXT5, false);
        using (WWW www = new WWW(dayTexUriBuildier.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(skySphereTexture);
        }

        skySphereTexture_2 = new Texture2D(8000, 2000, TextureFormat.DXT5, false);
        using (WWW www = new WWW(dayTexUriBuildier_2.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(skySphereTexture_2);
        }

        skySphereTexture_3 = new Texture2D(1, 1, TextureFormat.DXT5, false);
        using (WWW www = new WWW(dayTexUriBuildier_3.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(skySphereTexture_3);
        }

        skySphereNightTexture = new Texture2D(8192, 4096, TextureFormat.DXT5, false);
        using (WWW www = new WWW(nightTexUriBuilder.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(skySphereNightTexture);
        }

        treeTexture = new Texture2D(4096, 4096, TextureFormat.DXT5, false);
        using (WWW www = new WWW(treeTextureUriBuilder.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(treeTexture);
        }

        cactusTexture = new Texture2D(4096, 4096, TextureFormat.DXT5, false);
        using (WWW www = new WWW(cactusTextureUriBuilder.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(cactusTexture);
        }

        //Settings
        fog_enabled = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[0].Split(':')[1]);
        fogRed = float.Parse(File.ReadAllText(settingsFilePath).Split(']')[1].Split(':')[1]);
        fogBlue = float.Parse(File.ReadAllText(settingsFilePath).Split(']')[2].Split(':')[1]);
        fogGreen = float.Parse(File.ReadAllText(settingsFilePath).Split(']')[3].Split(':')[1]);
        fogDistance = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[4].Split(':')[1]);
        selectedSky = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[5].Split(':')[1]);
        terrainTexture = File.ReadAllText(settingsFilePath).Split(']')[6].Split(':')[1];
        surface_dust = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[7].Split(':')[1]);
        surface_mist = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[8].Split(':')[1]);
        dust_particles = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[9].Split(':')[1]);
        mist_cloud = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[10].Split(':')[1]);
        snow_mist = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[11].Split(':')[1]);
        snow_dust = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[12].Split(':')[1]);
        cold_mist = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[13].Split(':')[1]);
        toxic_mist = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[14].Split(':')[1]);

        //Effects
        sepia = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[0].Split(':')[1]);
        blur_bloom = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[1].Split(':')[1]);
        mars = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[2].Split(':')[1]);
        cell_shading = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[3].Split(':')[1]);
        gray_scale = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[4].Split(':')[1]);
        eight_bit = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[5].Split(':')[1]);
        arcade = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[6].Split(':')[1]);
        blur_focus = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[7].Split(':')[1]);
        night_vision = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[8].Split(':')[1]);
        colors_hsv = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[9].Split(':')[1]);
        chromatic = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[10].Split(':')[1]);
        old_tv = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[11].Split(':')[1]);
        hue_rotate = int.Parse(File.ReadAllText(effectsFilePath).Split(']')[12].Split(':')[1]);

        //Cheats
        disable_fall_damage = int.Parse(File.ReadAllText(cheatsFilePath).Split(']')[0].Split(':')[1]);
        jetpack = int.Parse(File.ReadAllText(cheatsFilePath).Split(']')[1].Split(':')[1]);
        toxic_filter = int.Parse(File.ReadAllText(cheatsFilePath).Split(']')[2].Split(':')[1]);
        ray_gun = int.Parse(File.ReadAllText(cheatsFilePath).Split(']')[3].Split(':')[1]);
        mk2_build_gun = int.Parse(File.ReadAllText(cheatsFilePath).Split(']')[4].Split(':')[1]);
        mk3_build_gun = int.Parse(File.ReadAllText(cheatsFilePath).Split(']')[5].Split(':')[1]);

        GetMistAndDust();
    }

    // Initializes the mod.
    private void Init()
    {
        RenderSettings.fog &= fog_enabled != 0;
        CreateSkySphere();
    }

    // Called once per frame.
    public void Update()
    {
        if (foundPlayer == false)
        {
            player = FindObjectOfType<LocalPlayerScript>();
            foundPlayer |= player != null;
        }
        else
        {
            if (gotPosition == false)
            {
                oX = player.mWorldX;
                oY = player.mWorldY + 20;
                oZ = player.mWorldZ;
                gotPosition = true;
            }
            player.mbDisableFallDamage |= disable_fall_damage == 1;
            player.mbHaveJetPack |= jetpack == 1;
            player.mbHasToxicFilter |= toxic_filter == 1;
            PlayerInventory.mbPlayerHasMK1RayGun |= ray_gun == 1;
            PlayerInventory.mbPlayerHasMK2BuildGun |= mk2_build_gun == 1;
            PlayerInventory.mbPlayerHasMK3BuildGun |= mk3_build_gun == 1;

            if (playerInventory == null)
            {
                playerInventory = WorldScript.mLocalPlayer.mInventory;
            }
            else
            {
                StackItems();
            }
        }

        if (mCam == null)
        {
            Camera[] allCams = Camera.allCameras;
            foreach (Camera c in allCams)
            {
                if (c != null)
                {
                    if (c.gameObject.name.Equals("Camera"))
                    {
                        mCam = c;
                    }
                }
            }
        }
        else
        {
            if (atmosphereInit == false)
            {
                Init();
            }
            if (skySphere != null)
            {
                ManageSkySphere();
            }

            SetCameraEffects();
        }

        if (guiEnabled == false)
        {
            if (Input.GetKeyDown(KeyCode.F10))
            {
                UIManager.AllowBuilding = false;
                UIManager.AllowInteracting = false;
                UIManager.AllowMovement = false;
                UIManager.CrossHairShown = false;
                UIManager.CursorShown = true;
                UIManager.GamePaused = true;
                UIManager.HotBarShown = false;
                UIManager.HudShown = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                guiEnabled = true;
            }
        }
        else
        {
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Mouse0))
            {
                UIManager.AllowBuilding = true;
                UIManager.AllowInteracting = true;
                UIManager.AllowMovement = true;
                UIManager.CrossHairShown = true;
                UIManager.CursorShown = false;
                UIManager.GamePaused = false;
                UIManager.HotBarShown = true;
                UIManager.HudShown = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                guiEnabled = false;
                effectsMenuOpen = false;
                settingsMenuOpen = false;
                cheatsMenuOpen = false;
                fogColorBoxOpen = false;
            }
        }

        SetFogValues();

        if (terrainTexture != "Normal" && lockGlowTubes == false)
        {
            ReplaceGlowTubes();
        }
    }

    private void StackItems()
    {
        ItemBase[,] items = playerInventory.maItemInventory;

        foreach (ItemBase thisItem in items)
        {
            if (thisItem != null)
            {
                if (thisItem.GetName().Equals("Rough Hewn Rock"))
                {
                    foreach (ItemBase otherItem in items)
                    {
                        if (otherItem != null)
                        {
                            if (otherItem.GetName().Equals("Rough Hewn Rock"))
                            {
                                if (thisItem.GetAmount() > 0 && otherItem.GetAmount() > 0)
                                {
                                    if (thisItem.GetAmount() + otherItem.GetAmount() < ItemManager.GetMaxStackSize(thisItem))
                                    {
                                        int count = thisItem.GetAmount() + otherItem.GetAmount();
                                        playerInventory.RemoveSpecificItem(thisItem);
                                        playerInventory.RemoveSpecificItem(otherItem);
                                        TerrainDataEntry terrainDataEntry = TerrainData.mEntries[199];
                                        playerInventory.CollectValue(199, terrainDataEntry.DefaultValue, count);
                                    }
                                }
                            }
                        }
                    }
                }
                if (thisItem.GetName().Equals("Organic Detritus"))
                {
                    foreach (ItemBase otherItem in items)
                    {
                        if (otherItem != null)
                        {
                            if (otherItem.GetName().Equals("Organic Detritus"))
                            {
                                if (thisItem.GetAmount() > 0 && otherItem.GetAmount() > 0)
                                {
                                    if (thisItem.GetAmount() + otherItem.GetAmount() < ItemManager.GetMaxStackSize(thisItem))
                                    {
                                        int count = thisItem.GetAmount() + otherItem.GetAmount();
                                        playerInventory.RemoveSpecificItem(thisItem);
                                        playerInventory.RemoveSpecificItem(otherItem);
                                        TerrainDataEntry terrainDataEntry = TerrainData.mEntries[17];
                                        playerInventory.CollectValue(17, terrainDataEntry.DefaultValue, count);
                                    }
                                }
                            }
                        }
                    }
                }
                if (thisItem.GetName().Equals("Snow"))
                {
                    foreach (ItemBase otherItem in items)
                    {
                        if (otherItem != null)
                        {
                            if (otherItem.GetName().Equals("Snow"))
                            {
                                if (thisItem.GetAmount() > 0 && otherItem.GetAmount() > 0)
                                {
                                    if (thisItem.GetAmount() + otherItem.GetAmount() < ItemManager.GetMaxStackSize(thisItem))
                                    {
                                        int count = thisItem.GetAmount() + otherItem.GetAmount();
                                        playerInventory.RemoveSpecificItem(thisItem);
                                        playerInventory.RemoveSpecificItem(otherItem);
                                        TerrainDataEntry terrainDataEntry = TerrainData.mEntries[21];
                                        playerInventory.CollectValue(21, terrainDataEntry.DefaultValue, count);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // Creates the sky sphere.
    private void CreateSkySphere()
    {
        skySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        skySphere.transform.position = mCam.gameObject.transform.position;
        skySphere.transform.localScale += new Vector3(512, 256, 512);
        ConvertNormals(skySphere);
        skySphere.GetComponent<Renderer>().material.mainTexture = skySphereTexture;
        skySphere.GetComponent<Renderer>().receiveShadows = false;
        skySphere.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        skySphere.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
        skySphere.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
        atmosphereInit = true;
    }

    // Recalculates sky sphere mesh normals so the texture is on the inside of the sphere.
    private void ConvertNormals(GameObject obj)
    {
        MeshFilter filter = obj.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }

    // Manages all aspects of the sky sphere at runtime.
    private void ManageSkySphere()
    {
        skySphere.transform.position = mCam.gameObject.transform.position;

        if (SurvivalWeatherManager.mbDayTime)
        {
            if (selectedSky == 1)
            {
                skySphere.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                skySphere.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                skySphere.GetComponent<Renderer>().material.mainTexture = skySphereTexture;
            }
            else if (selectedSky == 2)
            {
                skySphere.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                skySphere.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
                skySphere.GetComponent<Renderer>().material.mainTexture = skySphereTexture_2;
            }
            else
            {
                skySphere.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
                skySphere.GetComponent<Renderer>().material.EnableKeyword("_ALPHATEST_ON");
                skySphere.GetComponent<Renderer>().material.mainTexture = skySphereTexture_3;
            }
        }
        else if (selectedSky != 0)
        {
            skySphere.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
            skySphere.GetComponent<Renderer>().material.DisableKeyword("_ALPHATEST_ON");
            skySphere.GetComponent<Renderer>().material.mainTexture = skySphereNightTexture;
        }
        else
        {
            skySphere.GetComponent<Renderer>().material.shader = Shader.Find("Transparent/Diffuse");
            skySphere.GetComponent<Renderer>().material.EnableKeyword("_ALPHATEST_ON");
            skySphere.GetComponent<Renderer>().material.mainTexture = skySphereTexture_3;
        }

        if (mCam.transform.position.y < 0)
        {
            if (skySphere != null)
            {
                if (skySphere.GetComponent<Renderer>().enabled == true)
                {
                    skySphere.GetComponent<Renderer>().enabled = false;
                }
            }
            RenderSettings.fog = false;
        }
        else
        {
            if (skySphere.GetComponent<Renderer>().enabled == false)
            {
                skySphere.GetComponent<Renderer>().enabled = true;
            }
            if (fog_enabled == 1)
            {
                RenderSettings.fog = true;
            }
            else
            {
                RenderSettings.fog = false;
            }
        }
    }

    // Sets the fog distance and color.
    private void SetFogValues()
    {
        RenderSettings.fogStartDistance = 0;
        RenderSettings.fogEndDistance = fogDistance;
        RenderSettings.fogColor = new Color(fogRed, fogGreen, fogBlue, 1);
    }

    // Toggles all dust and mist renderers.
    private void GetMistAndDust()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.name == "Surface_Dust")
            {
                surfaceDust = obj;
                obj.GetComponent<Renderer>().enabled = surface_dust == 1;
            }
            if (obj.name == "Surface_Mist_Close")
            {
                surfaceMistClose = obj;
                obj.GetComponent<Renderer>().enabled = surface_mist == 1;
            }
            if (obj.name == "Dust_Particles")
            {
                dustParticles = obj;
                obj.GetComponent<Renderer>().enabled = dust_particles == 1;
            }
            if (obj.name == "MistCloud")
            {
                mistCloud = obj;
                obj.GetComponent<Renderer>().enabled = mist_cloud == 1;
            }
            if (obj.name == "Snow_Mist")
            {
                snowMist = obj;
                obj.GetComponent<Renderer>().enabled = snow_mist == 1;
            }
            if (obj.name == "Snow Dust New")
            {
                snowDust = obj;
                obj.GetComponent<Renderer>().enabled = snow_dust == 1;
            }
            if (obj.name == "Cold_Mist")
            {
                coldMist = obj;
                obj.GetComponent<Renderer>().enabled = cold_mist == 1;
            }
            if (obj.name == "Toxic_Mist")
            {
                toxicMist = obj;
                obj.GetComponent<Renderer>().enabled = toxic_mist == 1;
            }
        }
    }

    // Adds camera effects and enables them when toggled.
    private void SetCameraEffects()
    {
        if (mCam.gameObject.GetComponent<CameraFilterPack_Color_Sepia>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Color_Sepia>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_Blur_Bloom>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Blur_Bloom>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_TV_PlanetMars>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_TV_PlanetMars>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_Drawing_CellShading2>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Drawing_CellShading2>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_Color_GrayScale>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Color_GrayScale>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_FX_8bits>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_FX_8bits>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_TV_ARCADE>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_TV_ARCADE>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_Blur_Focus>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Blur_Focus>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_Oculus_NightVision1>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Oculus_NightVision1>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_Colors_HSV>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Colors_HSV>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Color_Chromatic_Aberration>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_TV_Old>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_TV_Old>();

        if (mCam.gameObject.GetComponent<CameraFilterPack_Colors_HUE_Rotate>() == null)
            mCam.gameObject.AddComponent<CameraFilterPack_Colors_HUE_Rotate>();

        mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled = sepia == 1;
        mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled = blur_bloom == 1;
        mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled = mars == 1;
        mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled = cell_shading == 1;
        mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled = gray_scale == 1;
        mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled = eight_bit == 1;
        mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled = arcade == 1;
        mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled = blur_focus == 1;
        mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled = night_vision == 1;
        mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled = colors_hsv == 1;
        mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled = chromatic == 1;
        mCam.GetComponent<CameraFilterPack_TV_Old>().enabled = old_tv == 1;
        mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled = hue_rotate == 1;
    }

    // Replaces all the glow tubes with trees or cactus.
    private void ReplaceGlowTubes()
    {
        InstanceManager[] managers = FindObjectsOfType<InstanceManager>();
        foreach (InstanceManager manager in managers)
        {
            MPB_Dual_Color_Instancer[] instancers = manager.maInstancers;
            if (instancers != null)
            {
                for (int i = 0; i < instancers.Length; i++)
                {
                    if (instancers[i] != null)
                    {
                        if (instancers[i].mMesh != null)
                        {
                            string meshName = instancers[i].mMesh.name;
                            if (meshName == "GlowTube01" || meshName == "GlowTube02" || meshName == "GlowTube03")
                            {
                                ObjImporter importer = new ObjImporter();
                                string getTerrainTexture = File.ReadAllText(settingsFilePath).Split(']')[6].Split(':')[1];
                                if (getTerrainTexture == "Grass" || getTerrainTexture == "Snow")
                                {
                                    Mesh newMesh = importer.ImportFile(treeFilePath);
                                    instancers[i].mMesh = newMesh;
                                    instancers[i].mMesh.name = "atmosphere_tree";
                                }
                                if (getTerrainTexture == "Dirt")
                                {
                                    Mesh newMesh = importer.ImportFile(cactusFilePath);
                                    instancers[i].mMesh = newMesh;
                                    instancers[i].mMesh.name = "atmosphere_cactus";
                                }
                            }
                        }
                    }
                }
            }

            Material[] materials = manager.maMaterials;
            if (materials != null)
            {
                for (int m = 0; m < materials.Length; m++)
                {
                    if (materials[m] != null)
                    {
                        string materialName = materials[m].name;
                        if (materialName == "GlowTubes_Instanced")
                        {
                            if (terrainTexture == "Grass" || terrainTexture == "Snow")
                                materials[m].SetTexture("_MainTex", treeTexture);

                            if (terrainTexture == "Dirt")
                                materials[m].SetTexture("_MainTex", cactusTexture);

                            materials[m].SetTexture("_BumpMap", null);
                            materials[m].SetTexture("_MetallicGlossMap", null);
                            materials[m].shader = Shader.Find("Standard");
                            materials[m].SetColor("_Color", Color.white);
                            materials[m].color = Color.white;
                        }
                    }
                }
            }
        }
    }

    private void SaveSettings()
    {
        File.WriteAllText(settingsFilePath, "fog_enabled:" + fog_enabled + "]fog_red:" + fogRed + "]fog_blue:" + fogBlue + "]fog_green:" + fogGreen + "]fog_distance:" + fogDistance + "]sky_texture:" + selectedSky + "]terrain_texture:" + terrainTexture + "]surface_dust:" + surface_dust + "]surface_mist:" + surface_mist + "]dust_particles:" + dust_particles + "]mist_cloud:" + mist_cloud + "]snow_mist:" + snow_mist + "]snow_dust:" + snow_dust + "]cold_mist:" + cold_mist + "]toxic_mist:" + toxic_mist);
    }

    private void SaveEffects()
    {
        File.WriteAllText(effectsFilePath, "sepia:" + sepia + "]blur_bloom:" + blur_bloom + "]mars:" + mars + "]cell_shading:" + cell_shading + "]gray_scale:" + gray_scale + "]eight_bit:" + eight_bit + "]arcade:" + arcade + "]blur_focus:" + blur_focus + "]night_vision:" + night_vision + "]colors_hsv:" + colors_hsv + "]chromatic:" + chromatic + "]old_tv:" + old_tv + "]hue_rotate:" + hue_rotate);
    }

    private void SaveCheats()
    {
        File.WriteAllText(cheatsFilePath, "disable_fall_damage:" + disable_fall_damage + "]jetpack:" + jetpack + "]toxic_filter:" + toxic_filter + "]ray_gun:" + ray_gun + "]mk2_build_gun:" + mk2_build_gun + "]mk3_build_gun:" + mk3_build_gun);
    }

    public void OnGUI()
    {
        //ASPECT RATIO
        int ScreenHeight = Screen.height;
        int ScreenWidth = Screen.width;

        //MESSAGES
        Rect messageRect = new Rect(((ScreenWidth / 2) - 300), ((ScreenHeight / 2) - 200), 600, 400);
        Rect messageLabelRect = new Rect(((ScreenWidth / 2) - 175), ((ScreenHeight / 2) - 100), 400, 100);
        Rect messageButtonRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.07f)), ((ScreenHeight / 2) + 30), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));

        //BACKGROUND
        Rect backgroundRect = new Rect((ScreenWidth * 0.08f), (ScreenHeight * 0.18f), (ScreenWidth * 0.80f), (ScreenHeight * 0.60f));

        //FOG MENU
        Rect fogColorBackgroundRect = new Rect(((ScreenWidth / 2) - 300), ((ScreenHeight / 2) - 200), 600, 400);
        Rect fogRedSliderRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.07f)), ((ScreenHeight / 2) - 60), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect fogGreenSliderRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.07f)), ((ScreenHeight / 2) - 20), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect fogBlueSliderRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.07f)), ((ScreenHeight / 2) + 20), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect fogRedSliderLabelRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.12f)), ((ScreenHeight / 2) - 66), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect fogGreenSliderLabelRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.12f)), ((ScreenHeight / 2) - 26), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect fogBlueSliderLabelRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.12f)), ((ScreenHeight / 2) + 14), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect fogSelectButtonRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.07f)), ((ScreenHeight / 2) + 60), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));

        //BUTTONS
        Rect button1Rect = new Rect((ScreenWidth * 0.20f), (ScreenHeight * 0.30f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button2Rect = new Rect((ScreenWidth * 0.20f), (ScreenHeight * 0.36f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button3Rect = new Rect((ScreenWidth * 0.20f), (ScreenHeight * 0.42f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button4Rect = new Rect((ScreenWidth * 0.20f), (ScreenHeight * 0.48f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button5Rect = new Rect((ScreenWidth * 0.20f), (ScreenHeight * 0.54f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button6Rect = new Rect((ScreenWidth * 0.40f), (ScreenHeight * 0.30f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button7Rect = new Rect((ScreenWidth * 0.40f), (ScreenHeight * 0.36f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button8Rect = new Rect((ScreenWidth * 0.40f), (ScreenHeight * 0.42f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button9Rect = new Rect((ScreenWidth * 0.40f), (ScreenHeight * 0.48f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button10Rect = new Rect((ScreenWidth * 0.40f), (ScreenHeight * 0.54f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button11Rect = new Rect((ScreenWidth * 0.60f), (ScreenHeight * 0.30f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button12Rect = new Rect((ScreenWidth * 0.60f), (ScreenHeight * 0.36f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button13Rect = new Rect((ScreenWidth * 0.60f), (ScreenHeight * 0.42f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));
        Rect button14Rect = new Rect((ScreenWidth * 0.65f), (ScreenHeight * 0.66f), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));

        if (guiEnabled == true)
        {
            if (settingsMenuOpen == false && effectsMenuOpen == false && cheatsMenuOpen == false)
            {
                GUI.color = Color.white;
                if (menuBackgroundTexture != null)
                {
                    GUI.DrawTexture(backgroundRect, menuBackgroundTexture);
                }
                GUI.color = Color.cyan;

                if (GUI.Button(button1Rect, "Settings"))
                {
                    if (settingsMenuOpen == false)
                    {
                        GetMistAndDust();
                        settingsMenuOpen = true;
                    }
                    else
                    {
                        settingsMenuOpen = false;
                    }
                }

                effectsMenuOpen |= GUI.Button(button2Rect, "Camera Effects");
                cheatsMenuOpen |= GUI.Button(button3Rect, "Cheats");

                if (GUI.Button(button14Rect, "CLOSE"))
                {
                    UIManager.AllowBuilding = true;
                    UIManager.AllowInteracting = true;
                    UIManager.AllowMovement = true;
                    UIManager.CrossHairShown = true;
                    UIManager.CursorShown = false;
                    UIManager.GamePaused = false;
                    UIManager.HotBarShown = true;
                    UIManager.HudShown = true;
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                    guiEnabled = false;
                    settingsMenuOpen = false;
                    effectsMenuOpen = false;
                    cheatsMenuOpen = false;
                    fogColorBoxOpen = false;
                }
            }

            if (settingsMenuOpen == true)
            {
                if (confirmBoxOpen == false)
                {
                    if (fogColorBoxOpen == false)
                    {
                        GUI.color = Color.white;
                        if (menuBackgroundTexture != null)
                        {
                            GUI.DrawTexture(backgroundRect, menuBackgroundTexture);
                        }
                        GUI.color = Color.cyan;

                        if (GUI.Button(button1Rect, "Sky Texture: " + selectedSky))
                        {
                            selectedSky = selectedSky < 2 ? selectedSky + 1 : 0;
                            SaveSettings();
                        }
                        if (GUI.Button(button2Rect, "Terrain: " + terrainTexture))
                        {
                            lockGlowTubes = true;
                            if (terrainTexture == "Snow")
                            {
                                File.Delete(terrainDataXmlString);
                                File.Copy(grassTerrainXmlString, terrainDataXmlString);
                                terrainTexture = "Grass";
                                if (restartConfirmed == false)
                                {
                                    confirmBoxOpen = true;
                                    restartConfirmed = true;
                                }
                            }
                            else if (terrainTexture == "Grass")
                            {
                                File.Delete(terrainDataXmlString);
                                File.Copy(dirtTerrainXmlString, terrainDataXmlString);
                                terrainTexture = "Dirt";
                                if (restartConfirmed == false)
                                {
                                    confirmBoxOpen = true;
                                    restartConfirmed = true;
                                }
                            }
                            else if (terrainTexture == "Dirt")
                            {
                                File.Delete(terrainDataXmlString);
                                terrainTexture = "Normal";
                                if (restartConfirmed == false)
                                {
                                    confirmBoxOpen = true;
                                    restartConfirmed = true;
                                }
                            }
                            else if (terrainTexture == "Normal")
                            {
                                File.Delete(terrainDataXmlString);
                                File.Copy(snowTerrainXmlString, terrainDataXmlString);
                                terrainTexture = "Snow";
                                if (restartConfirmed == false)
                                {
                                    confirmBoxOpen = true;
                                    restartConfirmed = true;
                                }
                            }
                            SaveSettings();
                        }
                        if (GUI.Button(button3Rect, "Surface Mist: " + Convert.ToBoolean(surface_mist)))
                        {
                            if (surfaceMistClose != null)
                            {
                                surfaceMistClose.GetComponent<Renderer>().enabled = !surfaceMistClose.GetComponent<Renderer>().enabled;
                                surface_mist = Convert.ToByte(surfaceMistClose.GetComponent<Renderer>().enabled);
                                SaveSettings();
                            }
                        }
                        if (GUI.Button(button4Rect, "Surface Dust: " + Convert.ToBoolean(surface_dust)))
                        {
                            if (surfaceDust != null)
                            {
                                surfaceDust.GetComponent<Renderer>().enabled = !surfaceDust.GetComponent<Renderer>().enabled;
                                surface_dust = Convert.ToByte(surfaceDust.GetComponent<Renderer>().enabled);
                                SaveSettings();
                            }                         
                        }
                        if (GUI.Button(button5Rect, "Mist Cloud: " + Convert.ToBoolean(mist_cloud)))
                        {
                            if (mistCloud != null)
                            {
                                mistCloud.GetComponent<Renderer>().enabled = !mistCloud.GetComponent<Renderer>().enabled;
                                mist_cloud = Convert.ToByte(mistCloud.GetComponent<Renderer>().enabled);
                                SaveSettings();
                            }
                        }
                        if (GUI.Button(button6Rect, "Snow Mist: " + Convert.ToBoolean(snow_mist)))
                        {
                            if (snowMist != null)
                            {
                                snowMist.GetComponent<Renderer>().enabled = !snowMist.GetComponent<Renderer>().enabled;
                                snow_mist = Convert.ToByte(snowMist.GetComponent<Renderer>().enabled);
                                SaveSettings();
                            }
                        }
                        if (GUI.Button(button7Rect, "Snow Dust: " + Convert.ToBoolean(snow_dust)))
                        {
                            if (snowDust != null)
                            {
                                snowDust.GetComponent<Renderer>().enabled = !snowDust.GetComponent<Renderer>().enabled;
                                snow_dust = Convert.ToByte(snowDust.GetComponent<Renderer>().enabled);
                                SaveSettings();
                            }
                        }
                        if (GUI.Button(button8Rect, "Cold Mist: " + Convert.ToBoolean(cold_mist)))
                        {
                            if (coldMist != null)
                            {
                                coldMist.GetComponent<Renderer>().enabled = !coldMist.GetComponent<Renderer>().enabled;
                                cold_mist = Convert.ToByte(coldMist.GetComponent<Renderer>().enabled);
                                SaveSettings();
                            }
                        }
                        if (GUI.Button(button9Rect, "Toxic Mist: " + Convert.ToBoolean(toxic_mist)))
                        {
                            if (toxicMist != null)
                            { 
                                toxicMist.GetComponent<Renderer>().enabled = !toxicMist.GetComponent<Renderer>().enabled;
                                toxic_mist = Convert.ToByte(toxicMist.GetComponent<Renderer>().enabled);
                                SaveSettings();
                            }
                        }
                        if (GUI.Button(button10Rect, "Dust Particles: " + Convert.ToBoolean(dust_particles)))
                        {
                            if (dustParticles != null)
                            {
                                dustParticles.GetComponent<Renderer>().enabled = !dustParticles.GetComponent<Renderer>().enabled;
                                dust_particles = Convert.ToByte(dustParticles.GetComponent<Renderer>().enabled);
                                SaveSettings();
                            }
                        }
                        if (GUI.Button(button11Rect, "Fog: " + Convert.ToBoolean(fog_enabled)))
                        {
                            fog_enabled = fog_enabled == 1 ? 0 : 1;
                            SaveSettings();
                        }
                        if (GUI.Button(button12Rect, "Fog Distance: " + fogDistance))
                        {
                            fogDistance = fogDistance < 250 ? fogDistance + 50 : 50;
                            SaveSettings();
                        }

                        fogColorBoxOpen |= GUI.Button(button13Rect, "Fog Color");

                        settingsMenuOpen &= !GUI.Button(button14Rect, "BACK");
                    }
                    else
                    {
                        GUI.color = Color.white;
                        if (menuBackgroundTexture != null)
                        {
                            GUI.DrawTexture(fogColorBackgroundRect, lightMenuBackgroundTexture);
                        }
                        GUI.color = Color.white;
                        GUI.Label(fogRedSliderLabelRect, "Red");
                        GUI.Label(fogGreenSliderLabelRect, "Green");
                        GUI.Label(fogBlueSliderLabelRect, "Blue");
                        GUI.color = Color.cyan;
                        fogRed = GUI.HorizontalSlider(fogRedSliderRect, fogRed, 0, 1);
                        fogGreen = GUI.HorizontalSlider(fogGreenSliderRect, fogGreen, 0, 1);
                        fogBlue = GUI.HorizontalSlider(fogBlueSliderRect, fogBlue, 0, 1);
                        fogColorBoxOpen &= !GUI.Button(fogSelectButtonRect, "SELECT");
                    }
                }
                else
                {
                    GUI.DrawTexture(messageRect, menuBackgroundTexture);
                    GUI.color = Color.white;
                    GUI.Label(messageLabelRect, "You will need to restart the game for this change to take affect.");
                    GUI.color = Color.cyan;
                    confirmBoxOpen &= !GUI.Button(messageButtonRect, "OK");
                }
            }

            if (cheatsMenuOpen == true)
            {
                GUI.color = Color.white;
                if (menuBackgroundTexture != null)
                {
                    GUI.DrawTexture(backgroundRect, menuBackgroundTexture);
                }
                GUI.color = Color.cyan;

                if (GUI.Button(button1Rect, "Disable Fall Damage: " + player.mbDisableFallDamage))
                {
                    player.mbDisableFallDamage = !player.mbDisableFallDamage;
                    disable_fall_damage = Convert.ToByte(player.mbDisableFallDamage);
                    SaveCheats();
                }
                if (GUI.Button(button2Rect, "Jetpack: " + player.mbHaveJetPack))
                {
                    player.mbHaveJetPack = !player.mbHaveJetPack;
                    jetpack = Convert.ToByte(player.mbHaveJetPack);
                    SaveCheats();
                }
                if (GUI.Button(button3Rect, "Toxic Filter: " + player.mbHasToxicFilter))
                {
                    player.mbHasToxicFilter = !player.mbHasToxicFilter;
                    toxic_filter = Convert.ToByte(player.mbHasToxicFilter);
                    SaveCheats();
                }
                if (GUI.Button(button4Rect, "MK1 Ray Gun: " + PlayerInventory.mbPlayerHasMK1RayGun))
                {
                    PlayerInventory.mbPlayerHasMK1RayGun = !PlayerInventory.mbPlayerHasMK1RayGun;
                    ray_gun = Convert.ToByte(PlayerInventory.mbPlayerHasMK1RayGun);
                    SaveCheats();
                }
                if (GUI.Button(button5Rect, "MK2 Build Gun: " + PlayerInventory.mbPlayerHasMK2BuildGun))
                {
                    PlayerInventory.mbPlayerHasMK2BuildGun = !PlayerInventory.mbPlayerHasMK2BuildGun;
                    mk2_build_gun = Convert.ToByte(PlayerInventory.mbPlayerHasMK2BuildGun);
                    SaveCheats();
                }
                if (GUI.Button(button6Rect, "MK3 Build Gun: " + PlayerInventory.mbPlayerHasMK3BuildGun))
                {
                    PlayerInventory.mbPlayerHasMK3BuildGun = !PlayerInventory.mbPlayerHasMK3BuildGun;
                    mk3_build_gun = Convert.ToByte(PlayerInventory.mbPlayerHasMK3BuildGun);
                    SaveCheats();
                }
                if (GUI.Button(button7Rect, "Teleport to CPH"))
                {
                    player.Teleport(oX, oY, oZ, mCam.gameObject.transform.forward, false);
                }

                cheatsMenuOpen &= !GUI.Button(button14Rect, "BACK");
            }

            if (effectsMenuOpen == true)
            {
                GUI.color = Color.white;
                if (menuBackgroundTexture != null)
                {
                    GUI.DrawTexture(backgroundRect, menuBackgroundTexture);
                }
                GUI.color = Color.cyan;

                if (GUI.Button(button1Rect, "Sepia: " + mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled = !mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled;
                    sepia = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button2Rect, "Bloom: " + mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled = !mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled;
                    blur_bloom = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button3Rect, "Mars: " + mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled = !mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled;
                    mars = Convert.ToByte(mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled); 
                    SaveEffects();
                }
                if (GUI.Button(button4Rect, "Cell Shading: " + mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled = !mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled;
                    cell_shading = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button5Rect, "Grayscale: " + mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled = !mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled;
                    gray_scale = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button6Rect, "8-bit: " + mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled = !mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled;
                    eight_bit = Convert.ToByte(mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button7Rect, "Arcade: " + mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled = !mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled;
                    arcade = Convert.ToByte(mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button8Rect, "Focused Blur: " + mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled = !mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled;
                    blur_focus = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button9Rect, "Night Vision: " + mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled = !mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled;
                    night_vision = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button10Rect, "Colors HSV: " + mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled = !mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled;
                    colors_hsv = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button11Rect, "Chromatic Aberration: " + mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled = !mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled;
                    chromatic = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button12Rect, "Old TV: " + mCam.GetComponent<CameraFilterPack_TV_Old>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_TV_Old>().enabled = !mCam.GetComponent<CameraFilterPack_TV_Old>().enabled;
                    old_tv = Convert.ToByte(mCam.GetComponent<CameraFilterPack_TV_Old>().enabled);
                    SaveEffects();
                }
                if (GUI.Button(button13Rect, "Hue Rotation: " + mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled))
                {
                    mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled = !mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled;
                    hue_rotate = Convert.ToByte(mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled);
                    SaveEffects();
                }

                effectsMenuOpen &= !GUI.Button(button14Rect, "BACK");
            }
        }
    }
}