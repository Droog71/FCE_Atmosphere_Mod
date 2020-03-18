using UnityEngine;
using System.Collections;
using System.IO;
using System.Reflection;
using System;

public class Atmosphere : FortressCraftMod
{
    //Sky sphere, textures and mist/dust objects.
    GameObject skySphere;
    GameObject surfaceMistClose;
    GameObject surfaceDust;
    GameObject mistCloud;
    GameObject dustParticles;
    GameObject snowMist;
    GameObject toxicMist;
    GameObject snowDust;
    GameObject coldMist;
    Texture2D menuBackgroundTexture;
    Texture2D skySphereTexture;
    Texture2D skySphereTexture_2;
    Texture2D skySphereNightTexture;
    Texture2D treeTexture;
    LocalPlayerScript player;

    //Settings
    int selectedSky = 1;
    int fog_enabled = 0;
    int fogColor = 1;
    int surface_dust = 0;
    int surface_mist = 0;
    int dust_particles = 0;
    int mist_cloud = 0;
    int snow_mist = 0;
    int snow_dust = 0;
    int cold_mist = 0;
    int toxic_mist = 0;
    string terrainTexture = "Snow";
    float fogDistance = 200;
    bool fog_enabled_display;

    //Effects
    int sepia = 0;
    int blur_bloom = 0;
    int mars = 0;
    int cell_shading = 0;
    int gray_scale = 0;
    int eight_bit = 0;
    int arcade = 0;
    int blur_focus = 0;
    int night_vision = 0;
    int colors_hsv = 0;
    int chromatic = 0;
    int old_tv = 0;
    int hue_rotate = 0;

    //Cheats
    int disable_fall_damage = 0;
    int jetpack = 0;
    int toxic_filter = 0;
    int ray_gun = 0;
    int mk2_build_gun = 0;
    int mk3_build_gun = 0;

    //Menus.
    bool guiEnabled;
    bool effectsMenuOpen;
    bool cheatsMenuOpen;
    bool settingsMenuOpen;
    bool confirmBoxOpen;

    //Player was found in the scene.
    bool foundPlayer;

    //Player position was found.
    bool gotPosition;

    //Player acknowledged restart required.
    bool restartConfirmed;

    //Player position.
    long oX = 0;
    long oY = 0;
    long oZ = 0;

    //The main camera.
    Camera mCam;

    //Init bool so we only iterate over scene objects once.
    bool atmosphereInit;

    //File locations.
    static readonly string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    static readonly string treeFilePath = Path.Combine(assemblyFolder, "Models/tree.obj");
    static readonly string settingsFilePath = Path.Combine(assemblyFolder, "settings.txt");
    static readonly string effectsFilePath = Path.Combine(assemblyFolder, "effects.txt");
    static readonly string cheatsFilePath = Path.Combine(assemblyFolder, "cheats.txt");
    static readonly string terrainDataXmlString = Path.Combine(assemblyFolder, "Xml/TerrainData.xml");
    static readonly string dirtTerrainXmlString = Path.Combine(assemblyFolder, "Xml/Dirt.xml");
    static readonly string grassTerrainXmlString = Path.Combine(assemblyFolder, "Xml/Grass.xml");
    static readonly string snowTerrainXmlString = Path.Combine(assemblyFolder, "Xml/Snow.xml");
    static readonly string menuBackgroundTextureString = Path.Combine(assemblyFolder, "Images/menu.png");
    static readonly string skySphereTextureString = Path.Combine(assemblyFolder, "Images/sky_day.jpg");
    static readonly string skySphereTextureString_2 = Path.Combine(assemblyFolder, "Images/sky_day_2.jpg");
    static readonly string skySphereNightTextureString = Path.Combine(assemblyFolder, "Images/sky_night.jpg");
    static readonly string treeTextureFilePath = Path.Combine(assemblyFolder, "Images/tree.jpg");
    UriBuilder menuTexUriBuildier = new UriBuilder(menuBackgroundTextureString);
    UriBuilder dayTexUriBuildier = new UriBuilder(skySphereTextureString);
    UriBuilder dayTexUriBuildier_2 = new UriBuilder(skySphereTextureString_2);
    UriBuilder nightTexUriBuilder = new UriBuilder(skySphereNightTextureString);
    UriBuilder treeTextureUriBuilder = new UriBuilder(treeTextureFilePath);

    //Mod registry.
    public override ModRegistrationData Register()
    {
        ModRegistrationData modRegistrationData = new ModRegistrationData();
        return modRegistrationData;
    }

    IEnumerator Start()
    {
        //Load textures.
        menuTexUriBuildier.Scheme = "file";
        dayTexUriBuildier.Scheme = "file";
        dayTexUriBuildier_2.Scheme = "file";
        nightTexUriBuilder.Scheme = "file";
        treeTextureUriBuilder.Scheme = "file";

        menuBackgroundTexture = new Texture2D(598, 358, TextureFormat.DXT5, false);
        using (WWW www = new WWW(menuTexUriBuildier.ToString()))
        {
            yield return www;
            www.LoadImageIntoTexture(menuBackgroundTexture);
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

        //Settings
        fog_enabled = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[0].Split(':')[1]);
        fogColor = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[1].Split(':')[1]);
        fogDistance = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[2].Split(':')[1]);
        selectedSky = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[3].Split(':')[1]);
        terrainTexture = File.ReadAllText(settingsFilePath).Split(']')[4].Split(':')[1];
        surface_dust = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[5].Split(':')[1]);
        surface_mist = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[6].Split(':')[1]);
        dust_particles = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[7].Split(':')[1]);
        mist_cloud = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[8].Split(':')[1]);
        snow_mist = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[9].Split(':')[1]);
        snow_dust = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[10].Split(':')[1]);
        cold_mist = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[11].Split(':')[1]);
        toxic_mist = int.Parse(File.ReadAllText(settingsFilePath).Split(']')[12].Split(':')[1]);

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
    }

    void Update()
    {
        //Find the main camera.
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

        //Force the fog distance to 200 and change it's color.
        if (RenderSettings.fog == true)
        {
            RenderSettings.fogStartDistance = 0;
            RenderSettings.fogEndDistance = fogDistance;
            if (fogColor == 1)
            {
                RenderSettings.fogColor = Color.white;
            }
            if (fogColor == 2)
            {
                RenderSettings.fogColor = new Color(0.9f, 0.9f, 0.9f, 1);
            }
            if (fogColor == 3)
            {
                RenderSettings.fogColor = new Color(0.8f, 0.8f, 0.8f, 1);
            }
            if (fogColor == 4)
            {
                RenderSettings.fogColor = new Color(0.7f, 0.7f, 0.7f, 1);
            }
            if (fogColor == 5)
            {
                RenderSettings.fogColor = new Color(0.6f, 0.6f, 0.6f, 1);
            }
            if (fogColor == 6)
            {
                RenderSettings.fogColor = Color.gray;
            }
        }

        //Change the color of snow mist.
        if (snowMist != null)
        {
            if (snowMist.GetComponent<Renderer>().enabled == true)
            {
                ParticleSystem.MainModule snowMistMain = snowMist.GetComponent<ParticleSystem>().main;
                snowMistMain.startColor = Color.white;
            }
        }

        //Find the player object.
        if (foundPlayer == false)
        {
            player = FindObjectOfType<LocalPlayerScript>();
            if (player != null)
            {
                foundPlayer = true;
            }
        }

        //Find the player's location.
        if (gotPosition == false)
        {
            oX = player.mWorldX;
            oY = player.mWorldY + 20;
            oZ = player.mWorldZ;
            gotPosition = true;
        }

        //The camera was found.
        if (mCam != null)
        {
            //Replace all the trees.
            ReplaceTrees();

            //Add effect components to the camera.
            if (mCam.gameObject.GetComponent<CameraFilterPack_Color_Sepia>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Color_Sepia>();
            }
            if (sepia == 1)
            {
                mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_Blur_Bloom>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Blur_Bloom>();
            }
            if (blur_bloom == 1)
            {
                mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_TV_PlanetMars>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_TV_PlanetMars>();
            }
            if (mars == 1)
            {
                mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_Drawing_CellShading2>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Drawing_CellShading2>();
            }
            if (cell_shading == 1)
            {
                mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_Color_GrayScale>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Color_GrayScale>();
            }
            if (gray_scale == 1)
            {
                mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_FX_8bits>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_FX_8bits>();
            }
            if (eight_bit == 1)
            {
                mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_TV_ARCADE>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_TV_ARCADE>();
            }
            if (arcade == 1)
            {
                mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_Blur_Focus>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Blur_Focus>();
            }
            if (blur_focus == 1)
            {
                mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_Oculus_NightVision1>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Oculus_NightVision1>();
            }
            if (night_vision == 1)
            {
                mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_Colors_HSV>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Colors_HSV>();
            }
            if (colors_hsv == 1)
            {
                mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Color_Chromatic_Aberration>();
            }
            if (chromatic == 1)
            {
                mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_TV_Old>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_TV_Old>();
            }
            if (old_tv == 1)
            {
                mCam.GetComponent<CameraFilterPack_TV_Old>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_TV_Old>().enabled = false;
            }

            if (mCam.gameObject.GetComponent<CameraFilterPack_Colors_HUE_Rotate>() == null)
            {
                mCam.gameObject.AddComponent<CameraFilterPack_Colors_HUE_Rotate>();
            }
            if (hue_rotate == 1)
            {
                mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled = true;
            }
            else
            {
                mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled = false;
            }

            if (atmosphereInit == false)
            {
                //Disable all dust, mist, cloud and orbiting object rendering.
                GameObject[] allObjects = FindObjectsOfType<GameObject>();
                foreach (GameObject obj in allObjects)
                {
                    //Disable all fog, mists, dusts and clouds that have not been activated.
                    if (fog_enabled == 0)
                    {
                        RenderSettings.fog = false;
                    }

                    if (obj.name == "Surface_Dust")
                    {
                        surfaceDust = obj;
                        if (surface_dust != 1)
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    if (obj.name == "Surface_Mist_Close")
                    {
                        surfaceMistClose = obj;
                        if (surface_mist != 1)
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    if (obj.name == "Dust_Particles")
                    {
                        dustParticles = obj;
                        if (dust_particles != 1)
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    if (obj.name == "MistCloud")
                    {
                        mistCloud = obj;
                        if (mist_cloud != 1)
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    if (obj.name == "Snow_Mist")
                    {
                        snowMist = obj;
                        if (snow_mist != 1)
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    if (obj.name == "Snow Dust New")
                    {
                        snowDust = obj;
                        if (snow_dust != 1)
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    if (obj.name == "Cold_Mist")
                    {
                        coldMist = obj;
                        if (cold_mist != 1)
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }
                    if (obj.name == "Toxic_Mist")
                    {
                        toxicMist = obj;
                        if (toxic_mist != 1)
                        {
                            obj.GetComponent<Renderer>().enabled = false;
                        }
                    }

                    //Prevent orbiting planet mesh from rendering inside sky sphere.
                    if (obj.name == "Large_Planet_44")
                    {
                        obj.GetComponent<Renderer>().enabled = false;
                    }
                    if (obj.name == "Small_Planet_30")
                    {
                        obj.GetComponent<Renderer>().enabled = false;
                    }
                    if (obj.name == "Large_Planet_30")
                    {
                        obj.GetComponent<Renderer>().enabled = false;
                    }
                    if (obj.name == "Small_Planet_31")
                    {
                        obj.GetComponent<Renderer>().enabled = false;
                    }
                    if (obj.name == "Moon_Object")
                    {
                        obj.GetComponent<Renderer>().enabled = false;
                    }
                }

                //Create the sky sphere.
                skySphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                skySphere.transform.position = mCam.gameObject.transform.position;
                skySphere.transform.localScale += new Vector3(512, 256, 512);
                ConvertNormals(skySphere);
                skySphere.GetComponent<Renderer>().material.mainTexture = skySphereTexture;
                skySphere.GetComponent<Renderer>().receiveShadows = false;
                skySphere.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                skySphere.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Texture");
                atmosphereInit = true;
            }

            if (skySphere != null)
            {
                //Keep the sky sphere centered on the camera.
                skySphere.transform.position = mCam.gameObject.transform.position;

                //Change the skybox texture at night.
                if (SurvivalWeatherManager.mbDayTime)
                {
                    if (selectedSky == 1)
                    {
                        skySphere.GetComponent<Renderer>().material.mainTexture = skySphereTexture;
                    }
                    else
                    {
                        skySphere.GetComponent<Renderer>().material.mainTexture = skySphereTexture_2;
                    }
                }
                else
                {
                    skySphere.GetComponent<Renderer>().material.mainTexture = skySphereNightTexture;
                }

                //Disable rendering of the sky sphere if the player is underground.
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

            //For fog toggle display in menu.
            if (fog_enabled == 1)
            {
                fog_enabled_display = true;
            }
            else
            {
                fog_enabled_display = false;
            }

            //Apply cheats
            if (disable_fall_damage == 1)
            {
                player.mbDisableFallDamage = true;
            }
            if (jetpack == 1)
            {
                player.mbHaveJetPack = true;
            }
            if (toxic_filter == 1)
            {
                player.mbHasToxicFilter = true;
            }
            if (ray_gun == 1)
            {
                PlayerInventory.mbPlayerHasMK1RayGun = true;
            }
            if (mk2_build_gun == 1)
            {
                PlayerInventory.mbPlayerHasMK2BuildGun = true;
            }
            if (mk3_build_gun == 1)
            {
                PlayerInventory.mbPlayerHasMK3BuildGun = true;
            }
        }

        if (guiEnabled == false)
        {
            //Open the menu.
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
                //Close the  menu.
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
            }
        }
    }

    //Recalculate sky sphere mesh normals so the texture is on the inside of the sphere.
    void ConvertNormals(GameObject obj)
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

    //Replaces all the glow tubes with trees.
    void ReplaceTrees()
    {
        InstanceManager[] managers = FindObjectsOfType<InstanceManager>();
        foreach (InstanceManager manager in managers)
        {
            MPB_Dual_Color_Instancer[] instancers = manager.maInstancers;
            int i = 0;
            while (i < instancers.Length)
            {
                string meshName = instancers[i].mMesh.name;
                if (meshName == "GlowTube01" || meshName == "GlowTube02" || meshName == "GlowTube03")
                {
                    ObjImporter importer = new ObjImporter();
                    Mesh newMesh = importer.ImportFile(treeFilePath);
                    instancers[i].mMesh = newMesh;
                    instancers[i].mMesh.name = "atmosphere_tree";
                }
                i++;
            }

            Material[] materials = manager.maMaterials;
            int m = 0;
            while (m < materials.Length)
            {
                string materialName = materials[m].name;
                if (materialName == "GlowTubes_Instanced")
                {
                    materials[m].SetTexture("_MainTex", treeTexture);
                    materials[m].SetTexture("_BumpMap", null);
                    materials[m].SetTexture("_MetallicGlossMap", null);
                    materials[m].shader = Shader.Find("Standard");
                    materials[m].SetColor("_Color", Color.white);
                    materials[m].color = Color.white;
                }
                m++;
            }
        }
    }

    void SaveSettings()
    {
        File.WriteAllText(settingsFilePath, "fog_enabled:" + fog_enabled + "]fog_color:" + fogColor + "]fog_distance:" + fogDistance + "]sky_texture:" + selectedSky + "]terrain_texture:" + terrainTexture + "]surface_dust:" + surface_dust + "]surface_mist:" + surface_mist + "]dust_particles:" + dust_particles + "]mist_cloud:" + mist_cloud + "]snow_mist:" + snow_mist + "]snow_dust:" + snow_dust + "]cold_mist:" + cold_mist + "]toxic_mist:" + toxic_mist);
    }

    void SaveEffects()
    {
        File.WriteAllText(effectsFilePath, "sepia:" + sepia + "]blur_bloom:" + blur_bloom + "]mars:" + mars + "]cell_shading:" + cell_shading + "]gray_scale:" + gray_scale + "]eight_bit:" + eight_bit + "]arcade:" + arcade + "]blur_focus:" + blur_focus + "]night_vision:" + night_vision + "]colors_hsv:" + colors_hsv + "]chromatic:" + chromatic + "]old_tv:" + old_tv + "]hue_rotate:" + hue_rotate);
    }

    void SaveCheats()
    {
        File.WriteAllText(cheatsFilePath, "disable_fall_damage:" + disable_fall_damage + "]jetpack:" + jetpack + "]toxic_filter:" + toxic_filter + "]ray_gun:" + ray_gun + "]mk2_build_gun:" + mk2_build_gun + "]mk3_build_gun:" + mk3_build_gun);
    }

    void OnGUI()
    {
        //ASPECT RATIO
        int ScreenHeight = Screen.height;
        int ScreenWidth = Screen.width;

        //MESSAGES
        Rect messageRect = new Rect(((ScreenWidth/2)-300), ((ScreenHeight/2)-200),600,400);
        Rect messageLabelRect = new Rect(((ScreenWidth / 2) - 175), ((ScreenHeight / 2) - 100), 400, 100);
        Rect messageButtonRect = new Rect(((ScreenWidth / 2) - (ScreenWidth * 0.07f)), ((ScreenHeight / 2) + 30), (ScreenWidth * 0.14f), (ScreenHeight * 0.05f));

        //BACKGROUND
        Rect backgroundRect = new Rect((ScreenWidth * 0.08f), (ScreenHeight * 0.18f), (ScreenWidth * 0.80f), (ScreenHeight * 0.60f));

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
            GUI.color = Color.white;
            if (menuBackgroundTexture != null)
            {
                GUI.DrawTexture(backgroundRect, menuBackgroundTexture);
            }
            GUI.color = Color.cyan;

            if (settingsMenuOpen == false && effectsMenuOpen == false && cheatsMenuOpen == false)
            {
                if (GUI.Button(button1Rect, "Settings"))
                {
                    settingsMenuOpen = true;
                }

                if (GUI.Button(button2Rect, "Camera Effects"))
                {
                    effectsMenuOpen = true;
                }

                if (GUI.Button(button3Rect, "Cheats"))
                {
                    cheatsMenuOpen = true;
                }

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
                }
            }

            if (settingsMenuOpen == true)
            {
                if (confirmBoxOpen == false)
                {
                    if (GUI.Button(button1Rect, "Daytime Sky Texture: " + selectedSky))
                    {
                        if (selectedSky == 1)
                        {
                            selectedSky = 2;
                            SaveSettings();
                        }
                        else
                        {
                            selectedSky = 1;
                            SaveSettings();
                        }
                    }
                    if (GUI.Button(button2Rect, "Terrain: " + terrainTexture))
                    {
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
                            File.Copy(snowTerrainXmlString, terrainDataXmlString);
                            terrainTexture = "Snow";
                            if (restartConfirmed == false)
                            {
                                confirmBoxOpen = true;
                                restartConfirmed = true;
                            }
                        }
                    }
                    if (GUI.Button(button3Rect, "Fog: " + fog_enabled_display))
                    {
                        if (fog_enabled == 1)
                        {
                            fog_enabled = 0;
                        }
                        else
                        {
                            fog_enabled = 1;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button4Rect, "Fog Color: " + fogColor))
                    {
                        if (fogColor < 6)
                        {
                            fogColor += 1;
                        }
                        else
                        {
                            fogColor = 1;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button5Rect, "Fog Distance: " + fogDistance))
                    {
                        if (fogDistance < 250)
                        {
                            fogDistance += 50;
                        }
                        else
                        {
                            fogDistance = 50;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button6Rect, "Surface Mist-Close: " + surfaceMistClose.GetComponent<Renderer>().enabled))
                    {
                        if (surfaceMistClose.GetComponent<Renderer>().enabled == false)
                        {
                            surfaceMistClose.GetComponent<Renderer>().enabled = true;
                            surface_mist = 1;
                        }
                        else
                        {
                            surfaceMistClose.GetComponent<Renderer>().enabled = false;
                            surface_mist = 0;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button7Rect, "Surface Dust: " + surfaceDust.GetComponent<Renderer>().enabled))
                    {
                        if (surfaceDust.GetComponent<Renderer>().enabled == false)
                        {
                            surfaceDust.GetComponent<Renderer>().enabled = true;
                            surface_dust = 1;
                        }
                        else
                        {
                            surfaceDust.GetComponent<Renderer>().enabled = false;
                            surface_dust = 0;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button8Rect, "Mist Cloud: " + mistCloud.GetComponent<Renderer>().enabled))
                    {
                        if (mistCloud.GetComponent<Renderer>().enabled == false)
                        {
                            mistCloud.GetComponent<Renderer>().enabled = true;
                            mist_cloud = 1;
                        }
                        else
                        {
                            mistCloud.GetComponent<Renderer>().enabled = false;
                            mist_cloud = 0;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button9Rect, "Snow Mist: " + snowMist.GetComponent<Renderer>().enabled))
                    {
                        if (snowMist.GetComponent<Renderer>().enabled == false)
                        {
                            snowMist.GetComponent<Renderer>().enabled = true;
                            snow_mist = 1;
                        }
                        else
                        {
                            snowMist.GetComponent<Renderer>().enabled = false;
                            snow_mist = 0;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button10Rect, "Snow Dust: " + snowDust.GetComponent<Renderer>().enabled))
                    {
                        if (snowDust.GetComponent<Renderer>().enabled == false)
                        {
                            snowDust.GetComponent<Renderer>().enabled = true;
                            snow_dust = 1;
                        }
                        else
                        {
                            snowDust.GetComponent<Renderer>().enabled = false;
                            snow_dust = 0;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button11Rect, "Cold Mist: " + coldMist.GetComponent<Renderer>().enabled))
                    {
                        if (coldMist.GetComponent<Renderer>().enabled == false)
                        {
                            coldMist.GetComponent<Renderer>().enabled = true;
                            cold_mist = 1;
                        }
                        else
                        {
                            coldMist.GetComponent<Renderer>().enabled = false;
                            cold_mist = 0;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button12Rect, "Toxic Mist: " + toxicMist.GetComponent<Renderer>().enabled))
                    {
                        if (toxicMist.GetComponent<Renderer>().enabled == false)
                        {
                            toxicMist.GetComponent<Renderer>().enabled = true;
                            toxic_mist = 1;
                        }
                        else
                        {
                            toxicMist.GetComponent<Renderer>().enabled = false;
                            toxic_mist = 0;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button13Rect, "Dust Particles: " + dustParticles.GetComponent<Renderer>().enabled))
                    {
                        if (dustParticles.GetComponent<Renderer>().enabled == false)
                        {
                            dustParticles.GetComponent<Renderer>().enabled = true;
                            dust_particles = 1;
                        }
                        else
                        {
                            dustParticles.GetComponent<Renderer>().enabled = false;
                            dust_particles = 0;
                        }
                        SaveSettings();
                    }
                    if (GUI.Button(button14Rect, "BACK"))
                    {
                        settingsMenuOpen = false;
                    }
                }
                else
                {
                    GUI.DrawTexture(messageRect, menuBackgroundTexture);
                    GUI.color = Color.white;
                    GUI.Label(messageLabelRect, "You will need to restart the game for this change to take affect.");
                    GUI.color = Color.cyan;
                    if (GUI.Button(messageButtonRect,"OK"))
                    {
                        confirmBoxOpen = false;
                    }
                }
            }

            if (cheatsMenuOpen == true)
            {
                if (GUI.Button(button1Rect, "Disable Fall Damage: " + player.mbDisableFallDamage))
                {
                    if (player.mbDisableFallDamage == true)
                    {
                        player.mbDisableFallDamage = false;
                        disable_fall_damage = 0;
                    }
                    else
                    {
                        player.mbDisableFallDamage = true;
                        disable_fall_damage = 1;
                    }
                    SaveCheats();
                }
                if (GUI.Button(button2Rect, "Jetpack: " + player.mbHaveJetPack))
                {
                    if (player.mbHaveJetPack == true)
                    {
                        player.mbHaveJetPack = false;
                        jetpack = 0;
                    }
                    else
                    {
                        player.mbHaveJetPack = true;
                        jetpack = 1;
                    }
                    SaveCheats();
                }
                if (GUI.Button(button3Rect, "Toxic Filter: " + player.mbHasToxicFilter))
                {
                    if (player.mbHasToxicFilter == true)
                    {
                        player.mbHasToxicFilter = false;
                        toxic_filter = 0;
                    }
                    else
                    {
                        player.mbHasToxicFilter = true;
                        toxic_filter = 1;
                    }
                    SaveCheats();
                }

                if (GUI.Button(button4Rect, "MK1 Ray Gun: " + PlayerInventory.mbPlayerHasMK1RayGun))
                {
                    if (PlayerInventory.mbPlayerHasMK1RayGun == true)
                    {
                        PlayerInventory.mbPlayerHasMK1RayGun = false;
                        ray_gun = 0;
                    }
                    else
                    {
                        PlayerInventory.mbPlayerHasMK1RayGun = true;
                        ray_gun = 1;
                    }
                    SaveCheats();
                }

                if (GUI.Button(button5Rect, "MK2 Build Gun: " + PlayerInventory.mbPlayerHasMK2BuildGun))
                {
                    if (PlayerInventory.mbPlayerHasMK2BuildGun == true)
                    {
                        PlayerInventory.mbPlayerHasMK2BuildGun = false;
                        mk2_build_gun = 0;
                    }
                    else
                    {
                        PlayerInventory.mbPlayerHasMK2BuildGun = true;
                        mk2_build_gun = 1;
                    }
                    SaveCheats();
                }

                if (GUI.Button(button6Rect, "MK3 Build Gun: " + PlayerInventory.mbPlayerHasMK3BuildGun))
                {
                    if (PlayerInventory.mbPlayerHasMK3BuildGun == true)
                    {
                        PlayerInventory.mbPlayerHasMK3BuildGun = false;
                        mk3_build_gun = 0;
                    }
                    else
                    {
                        PlayerInventory.mbPlayerHasMK3BuildGun = true;
                        mk3_build_gun = 1;
                    }
                    SaveCheats();
                }

                if (GUI.Button(button7Rect, "Teleport to CPH"))
                {
                    player.Teleport(oX, oY, oZ, mCam.gameObject.transform.forward, false);
                }
                if (GUI.Button(button14Rect, "BACK"))
                {
                    cheatsMenuOpen = false;
                }
            }

            if (effectsMenuOpen == true)
            {
                if (GUI.Button(button1Rect, "Sepia: " + mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled = false;
                        sepia = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Color_Sepia>().enabled = true;
                        sepia = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button2Rect, "Bloom: " + mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled = false;
                        blur_bloom = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Blur_Bloom>().enabled = true;
                        blur_bloom = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button3Rect, "Mars: " + mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled = false;
                        mars = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_TV_PlanetMars>().enabled = true;
                        mars = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button4Rect, "Cell Shading: " + mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled = false;
                        cell_shading = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Drawing_CellShading2>().enabled = true;
                        cell_shading = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button5Rect, "Grayscale: " + mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled = false;
                        gray_scale = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Color_GrayScale>().enabled = true;
                        gray_scale = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button6Rect, "8-bit: " + mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled = false;
                        eight_bit = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_FX_8bits>().enabled = true;
                        eight_bit = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button7Rect, "Arcade: " + mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled = false;
                        arcade = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_TV_ARCADE>().enabled = true;
                        arcade = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button8Rect, "Focused Blur: " + mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled = false;
                        blur_focus = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Blur_Focus>().enabled = true;
                        blur_focus = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button9Rect, "Night Vision: " + mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled = false;
                        night_vision = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Oculus_NightVision1>().enabled = true;
                        night_vision = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button10Rect, "Colors HSV: " + mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled = false;
                        colors_hsv = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Colors_HSV>().enabled = true;
                        colors_hsv = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button11Rect, "Chromatic Aberration: " + mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled = false;
                        chromatic = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Color_Chromatic_Aberration>().enabled = true;
                        chromatic = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button12Rect, "Old TV: " + mCam.GetComponent<CameraFilterPack_TV_Old>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_TV_Old>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_TV_Old>().enabled = false;
                        old_tv = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_TV_Old>().enabled = true;
                        old_tv = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button13Rect, "Hue Rotation: " + mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled))
                {
                    if (mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled == true)
                    {
                        mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled = false;
                        hue_rotate = 0;
                    }
                    else
                    {
                        mCam.GetComponent<CameraFilterPack_Colors_HUE_Rotate>().enabled = true;
                        hue_rotate = 1;
                    }
                    SaveEffects();
                }
                if (GUI.Button(button14Rect, "BACK"))
                {
                    effectsMenuOpen = false;
                }
            }
        }
    }
}