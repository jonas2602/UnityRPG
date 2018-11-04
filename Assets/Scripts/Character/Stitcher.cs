using UnityEngine;
using System.Collections.Generic;

public class Stitcher
{
    /// <summary>
    /// Stitch clothing onto an avatar.  Both clothing and avatar must be instantiated however clothing may be destroyed after.
    /// </summary>
    /// <param name="sourceClothing"></param>
    /// <param name="targetAvatar"></param>
    /// <returns>Newly created clothing on avatar</returns>
    public GameObject Stitch(GameObject sourceClothing, Transform targetAvatar)
    {
        // scan avatar for bones
        TransformCatelog boneCatelog = new TransformCatelog(targetAvatar);
        // get skinnedMeshRenderers of clothPrefab
        SkinnedMeshRenderer[] skinnedMeshRenderers = sourceClothing.GetComponentsInChildren<SkinnedMeshRenderer>();
        // parented cloth
        GameObject targetOfClothing = AddChild(sourceClothing, targetAvatar);
        foreach (SkinnedMeshRenderer clothRenderer in skinnedMeshRenderers)
        {
            // create skinnedMeshRenderer on base of prefab
            SkinnedMeshRenderer targetRenderer = AddSkinnedMeshRenderer(clothRenderer, targetOfClothing);
            // add transforms of avatar to RendererBones
            targetRenderer.bones = TranslateTransforms(clothRenderer.bones, boneCatelog);
        }
        return targetOfClothing;

    }

    // add parent of avatar to cloth
    private GameObject AddChild(GameObject source, Transform parent)
    {
        var target = new GameObject(source.name);
        target.transform.parent = parent;
        target.transform.localPosition = source.transform.localPosition;
        target.transform.localRotation = source.transform.localRotation;
        target.transform.localScale = source.transform.localScale;
        return target;
    }
    // initialisate skinnedMeshRenderer on cloth
    private SkinnedMeshRenderer AddSkinnedMeshRenderer(SkinnedMeshRenderer source, GameObject parent)
    {
        // add meshRenderer to cloth
        SkinnedMeshRenderer target = parent.AddComponent<SkinnedMeshRenderer>();
        // add mesh to meshRenderer
        target.sharedMesh = source.sharedMesh;
        // add materials to meshRenderer
        target.materials = source.materials;
        return target;
    }
    //add transforms of avatar to bones
    private Transform[] TranslateTransforms(Transform[] sources, TransformCatelog transformCatelog)
    {
        Transform[] targets = new Transform[sources.Length];
        for (int index = 0; index < sources.Length; index++)
            targets[index] = DictionaryExtensions.Find(transformCatelog, sources[index].name);
        return targets;
    }


    #region TransformCatelog
    private class TransformCatelog : Dictionary<string, Transform>
    {
        #region Constructors
        public TransformCatelog(Transform transform)
        {
            Catelog(transform);
        }
        #endregion

        #region Catelog
        private void Catelog(Transform transform)
        {
            // Debug.Log("Name: " + transform.name + "Bone: " + transform);
            Add(transform.name, transform);
            foreach (Transform child in transform)
                Catelog(child);
        }
        #endregion
    }
    #endregion


    #region DictionaryExtensions
    private class DictionaryExtensions
    {
        public static TValue Find<TKey, TValue>(Dictionary<TKey, TValue> source, TKey key)
        {
            TValue value;
            source.TryGetValue(key, out value);
            return value;
        }
    }
    #endregion

}
