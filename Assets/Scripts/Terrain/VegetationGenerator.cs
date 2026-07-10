using UnityEngine;

public static class VegetationGenerator
{
    public static void Generate(Terrain terrain, BiomeConfig config, int seed, Transform parent)
    {
        if (terrain == null || parent == null) return;

        TerrainData data = terrain.terrainData;
        Vector3 size = data.size;
        System.Random rng = new System.Random(seed);

        int treeCount = Mathf.RoundToInt(config.vegetationDensity * size.x * size.z * 0.015f);
        int rockCount = Mathf.RoundToInt(config.rockFrequency * size.x * size.z * 0.008f);
        int bushCount = config.vegetationType != VegetationStyle.Cacti
            ? Mathf.RoundToInt(config.vegetationDensity * size.x * size.z * 0.01f)
            : Mathf.RoundToInt(config.rockFrequency * size.x * size.z * 0.01f);

        for (int i = 0; i < treeCount; i++)
        {
            float x = (float)rng.NextDouble() * size.x * 0.9f + size.x * 0.05f;
            float z = (float)rng.NextDouble() * size.z * 0.9f + size.z * 0.05f;
            Vector3 pos = new Vector3(x, terrain.SampleHeight(new Vector3(x, 0, z)), z);
            CreateTree(parent, pos, config);
        }

        for (int i = 0; i < rockCount; i++)
        {
            float x = (float)rng.NextDouble() * size.x * 0.9f + size.x * 0.05f;
            float z = (float)rng.NextDouble() * size.z * 0.9f + size.z * 0.05f;
            Vector3 pos = new Vector3(x, terrain.SampleHeight(new Vector3(x, 0, z)), z);
            CreateRock(parent, pos, config);
        }

        for (int i = 0; i < bushCount; i++)
        {
            float x = (float)rng.NextDouble() * size.x * 0.9f + size.x * 0.05f;
            float z = (float)rng.NextDouble() * size.z * 0.9f + size.z * 0.05f;
            Vector3 pos = new Vector3(x, terrain.SampleHeight(new Vector3(x, 0, z)), z);
            CreateBush(parent, pos, config);
        }
    }

    public static GameObject CreateTree(Transform parent, Vector3 position, BiomeConfig config)
    {
        GameObject tree = new GameObject("Tree");
        tree.transform.SetParent(parent);
        tree.transform.position = position;

        switch (config.vegetationType)
        {
            case VegetationStyle.ShortRounded:
                BuildShortRoundedTree(tree.transform, config);
                break;
            case VegetationStyle.TallDense:
                BuildTallDenseTree(tree.transform, config);
                break;
            case VegetationStyle.Pine:
                BuildPineTree(tree.transform, config);
                break;
            case VegetationStyle.Cacti:
                BuildCacti(tree.transform, config);
                break;
            case VegetationStyle.Mangrove:
                BuildMangroveTree(tree.transform, config);
                break;
            case VegetationStyle.Palm:
                BuildPalmTree(tree.transform, config);
                break;
            default:
                BuildShortRoundedTree(tree.transform, config);
                break;
        }

        tree.isStatic = true;
        foreach (Transform child in tree.transform)
            child.gameObject.isStatic = true;

        return tree;
    }

    public static GameObject CreateRock(Transform parent, Vector3 position, BiomeConfig config)
    {
        GameObject rock = new GameObject("Rock");
        rock.transform.SetParent(parent);
        rock.transform.position = position;

        GameObject main = GameObject.CreatePrimitive(PrimitiveType.Cube);
        main.transform.SetParent(rock.transform);
        main.transform.localPosition = Vector3.zero;
        main.transform.localScale = new Vector3(0.8f, 0.4f, 0.6f);
        main.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(config.cliffColor);

        rock.isStatic = true;
        main.isStatic = true;
        return rock;
    }

    public static GameObject CreateBush(Transform parent, Vector3 position, BiomeConfig config)
    {
        GameObject bush = new GameObject("Bush");
        bush.transform.SetParent(parent);
        bush.transform.position = position;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.SetParent(bush.transform);
        sphere.transform.localPosition = new Vector3(0, 0.3f, 0);
        sphere.transform.localScale = new Vector3(0.6f, 0.5f, 0.6f);
        sphere.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(config.groundColorAlt);

        bush.isStatic = true;
        sphere.isStatic = true;
        return bush;
    }

    private static void BuildShortRoundedTree(Transform root, BiomeConfig config)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(root);
        trunk.transform.localPosition = new Vector3(0, 1f, 0);
        trunk.transform.localScale = new Vector3(0.25f, 1f, 0.25f);
        trunk.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(new Color(0.45f, 0.3f, 0.15f));

        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        leaves.transform.SetParent(root);
        leaves.transform.localPosition = new Vector3(0, 2.2f, 0);
        leaves.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        leaves.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(config.groundColor);
    }

    private static void BuildTallDenseTree(Transform root, BiomeConfig config)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
        trunk.transform.SetParent(root);
        trunk.transform.localPosition = new Vector3(0, 2f, 0);
        trunk.transform.localScale = new Vector3(0.35f, 4f, 0.35f);
        trunk.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(new Color(0.4f, 0.28f, 0.12f));

        float[] leafSizes = { 2f, 1.6f, 1.2f };
        float[] leafHeights = { 3f, 3.8f, 4.4f };
        for (int i = 0; i < leafSizes.Length; i++)
        {
            GameObject leaf = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leaf.transform.SetParent(root);
            leaf.transform.localPosition = new Vector3(0, leafHeights[i], 0);
            leaf.transform.localScale = new Vector3(leafSizes[i], 0.6f, leafSizes[i]);
            Color c = Color.Lerp(config.groundColor, config.groundColorAlt, i * 0.3f);
            leaf.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(c);
        }
    }

    private static void BuildPineTree(Transform root, BiomeConfig config)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(root);
        trunk.transform.localPosition = new Vector3(0, 1.5f, 0);
        trunk.transform.localScale = new Vector3(0.2f, 1.5f, 0.2f);
        trunk.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(new Color(0.35f, 0.25f, 0.15f));

        float[] coneScales = { 0.8f, 0.6f, 0.4f };
        float[] coneHeights = { 2.2f, 2.8f, 3.2f };
        for (int i = 0; i < coneScales.Length; i++)
        {
            GameObject cone = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cone.transform.SetParent(root);
            cone.transform.localPosition = new Vector3(0, coneHeights[i], 0);
            cone.transform.localScale = new Vector3(coneScales[i], 0.4f, coneScales[i]);
            Color c = Color.Lerp(new Color(0.15f, 0.35f, 0.2f), new Color(0.2f, 0.4f, 0.25f), i * 0.3f);
            cone.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(c);
        }
    }

    private static void BuildCacti(Transform root, BiomeConfig config)
    {
        GameObject stem = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        stem.transform.SetParent(root);
        stem.transform.localPosition = new Vector3(0, 0.8f, 0);
        stem.transform.localScale = new Vector3(0.15f, 0.8f, 0.15f);
        stem.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(new Color(0.4f, 0.55f, 0.35f));

        GameObject top = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        top.transform.SetParent(root);
        top.transform.localPosition = new Vector3(0, 1.4f, 0);
        top.transform.localScale = new Vector3(0.25f, 0.3f, 0.25f);
        top.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(new Color(0.35f, 0.5f, 0.3f));
    }

    private static void BuildMangroveTree(Transform root, BiomeConfig config)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(root);
        trunk.transform.localPosition = new Vector3(0, 0.6f, 0);
        trunk.transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);
        trunk.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(new Color(0.5f, 0.35f, 0.2f));

        GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Cube);
        leaves.transform.SetParent(root);
        leaves.transform.localPosition = new Vector3(0, 1.2f, 0);
        leaves.transform.localScale = new Vector3(1f, 0.5f, 1f);
        leaves.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(config.groundColor);
    }

    private static void BuildPalmTree(Transform root, BiomeConfig config)
    {
        GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        trunk.transform.SetParent(root);
        trunk.transform.localPosition = new Vector3(0, 2f, 0);
        trunk.transform.localScale = new Vector3(0.15f, 2f, 0.15f);
        trunk.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(new Color(0.55f, 0.45f, 0.3f));

        GameObject frond = GameObject.CreatePrimitive(PrimitiveType.Quad);
        frond.transform.SetParent(root);
        frond.transform.localPosition = new Vector3(0, 3.5f, 0);
        frond.transform.localScale = new Vector3(1.5f, 0.1f, 2f);
        frond.GetComponent<Renderer>().material = ShaderHelper.CreateMaterial(config.groundColor);
    }
}
