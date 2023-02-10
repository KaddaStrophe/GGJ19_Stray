using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;

#region Unity
public static class Utility
{
    #region Transforms

    public static void MoveGameObject(Transform transform, Vector3 trgPos, float duration, MonoBehaviour monoBehaviour, AnimationCurve animationCurve = null)
    {
        monoBehaviour.StartCoroutine(MoveGameObjectRoutine(transform, trgPos, duration, animationCurve));
    }
    public static IEnumerator MoveGameObjectRoutine(Transform transform, Vector3 trgPos, float duration, AnimationCurve animationCurve = null)
    {
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        if (steps < 2) steps = 2;
        float progress = 0f;
        Vector3 startPos = transform.position;
        for (int i = 0; i < steps; i++)
        {
            progress = Remap(i, 0, steps - 1, 0f, 1f);
            if (animationCurve != null) progress = animationCurve.Evaluate(progress);
            transform.position = Vector3.Lerp(startPos, trgPos, progress);
            yield return new WaitForFixedUpdate();
        }
    }

    public static void RotateGameObject(Transform transform, Quaternion trgRotation, float duration, MonoBehaviour monoBehaviour, AnimationCurve animationCurve = null)
    {
        monoBehaviour.StartCoroutine(RotateGameObjectRoutine(transform, trgRotation, duration, animationCurve));
    }
    public static IEnumerator RotateGameObjectRoutine(Transform transform, Quaternion trgRotation, float duration, AnimationCurve animationCurve)
    {
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        if (steps < 2) steps = 2;
        float progress = 0f;
        Quaternion startRotation = transform.rotation;
        for (int i = 0; i < steps; i++)
        {
            progress = Remap(i, 0, steps - 1, 0f, 1f);
            if (animationCurve != null) progress = animationCurve.Evaluate(progress);
            transform.rotation = Quaternion.Lerp(startRotation, trgRotation, progress);
            yield return new WaitForFixedUpdate();
        }
    }

    public static void ScaleGameObject(Transform transform, Vector3 targetScale, float duration, MonoBehaviour monoBehaviour, AnimationCurve animationCurve = null)
    {
        monoBehaviour.StartCoroutine(ScaleGameObjectRoutine(transform, targetScale, duration, animationCurve));
    }
    public static IEnumerator ScaleGameObjectRoutine(Transform transform, Vector3 targetScale, float duration, AnimationCurve animationCurve = null)
    {
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        if (steps < 2) steps = 2;
        float progress = 0f;
        Vector3 startScale = transform.localScale;
        for (int i = 0; i < steps; i++)
        {
            progress = Remap(i, 0, steps - 1, 0f, 1f);
            if (animationCurve != null) progress = animationCurve.Evaluate(progress);
            transform.localScale = Vector3.Lerp(startScale, targetScale, progress);
            yield return new WaitForFixedUpdate();
        }
    }

    #endregion

    #region Image/Sprite/Text/Materials
    public static void SimpleAnimate(SpriteRenderer spriteRend, float intervall, MonoBehaviour monoBehaviour, params Sprite[] sprites)
    {
        monoBehaviour.StartCoroutine(SimpleAnimateCR(spriteRend, intervall, sprites));
    }
    private static IEnumerator SimpleAnimateCR(SpriteRenderer spriteRend, float intervall, params Sprite[] sprites)
    {
        int index = 0;
        while (true)
        {
            spriteRend.sprite = sprites[index];
            ++index;
            if (index > sprites.Length - 1) index = 0;
            yield return new WaitForSeconds(intervall);
        }
    }

    public static void LerpColor(Image image, Color targetColor, float duration, bool ignoreTimeScale, MonoBehaviour monoBehaviour, AnimationCurve animationCurve = null)
    {
        monoBehaviour.StartCoroutine(LerpColorRoutine(image, targetColor, duration, ignoreTimeScale, animationCurve));
    }
    public static IEnumerator LerpColorRoutine(Image image, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null)
    {
        image.gameObject.SetActive(true);
        image.enabled = true;
        Color startingColor = image.color;
        float progress = 0f;
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);

        if (ignoreTimeScale)
        {
            float increment = duration / (float)steps;
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                image.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForSecondsRealtime(increment);
            }
        }
        else
        {
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                image.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public static void LerpColor(SpriteRenderer rend, Color targetColor, float duration, bool ignoreTimeScale, MonoBehaviour monoBehaviour, AnimationCurve animationCurve = null)
    {
        monoBehaviour.StartCoroutine(LerpColorRoutine(rend, targetColor, duration, ignoreTimeScale, animationCurve));
    }
    public static IEnumerator LerpColorRoutine(SpriteRenderer rend, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null)
    {
        Color startingColor = rend.color;
        float progress = 0f;
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        float increment = duration / (float)steps;
        if (ignoreTimeScale)
        {
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                rend.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForSecondsRealtime(increment);
            }
        }
        else
        {
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                rend.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public static void LerpColor(TMPro.TextMeshProUGUI textMesh, Color targetColor, float duration, bool ignoreTimeScale, MonoBehaviour monoBehaviour, AnimationCurve animationCurve = null)
    {
        monoBehaviour.StartCoroutine(LerpColorRoutine(textMesh, targetColor, duration, ignoreTimeScale, animationCurve));
    }
    public static IEnumerator LerpColorRoutine(TMPro.TextMeshProUGUI textMesh, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null)
    {
        Color startingColor = textMesh.color;
        float progress = 0f;
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        if (ignoreTimeScale)
        {
            float increment = duration / (float)steps;
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                textMesh.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForSecondsRealtime(increment);
            }
        }
        else
        {
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                textMesh.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public static void LerpColor(TMPro.TextMeshPro textMesh, Color targetColor, float duration, bool ignoreTimeScale, MonoBehaviour monoBehaviour, AnimationCurve animationCurve = null)
    {
        monoBehaviour.StartCoroutine(LerpColorRoutine(textMesh, targetColor, duration, ignoreTimeScale, animationCurve));
    }
    public static IEnumerator LerpColorRoutine(TMPro.TextMeshPro textMesh, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null)
    {
        Color startingColor = textMesh.color;
        float progress = 0f;
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);

        if (ignoreTimeScale)
        {
            float increment = duration / (float)steps;
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                textMesh.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForSecondsRealtime(increment);
            }
        }
        else
        {
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                textMesh.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    public static void LerpMaterialColor(Material mat, Color targetColor, float duration, bool ignoreTimeScale, MonoBehaviour monoBehaviour, AnimationCurve animationCurve = null)
    {
        monoBehaviour.StartCoroutine(LerpMaterialColorRoutine(mat, targetColor, duration, ignoreTimeScale, animationCurve));

    }
    public static IEnumerator LerpMaterialColorRoutine(Material mat, Color targetColor, float duration, bool ignoreTimeScale, AnimationCurve animationCurve = null)
    {
        Color startingColor = mat.color;
        float progress = 0f;
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);

        if (ignoreTimeScale)
        {
            float increment = duration / (float)steps;
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                mat.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForSecondsRealtime(increment);
            }
        }
        else
        {
            for (int i = 0; i < steps; i++)
            {
                progress = Remap(i, 0, steps - 1, 0f, 1f);
                if (animationCurve != null) progress = animationCurve.Evaluate(progress);
                mat.color = Color.Lerp(startingColor, targetColor, progress);
                yield return new WaitForFixedUpdate();
            }
        }
    }

    #endregion

    #region Audio
    public static void AdjustAudioVolume(AudioSource source, float targetVolume, float duration, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(AdjustAudioVolumeCR(source, targetVolume, duration));
    }
    public static IEnumerator AdjustAudioVolumeCR(AudioSource source, float targetVolume, float duration)
    {
        float startingValue = source.volume;
        int steps = Mathf.RoundToInt(duration / Time.fixedDeltaTime);
        float progress = 0f;
        for (int i = 0; i < steps; i++)
        {
            progress = Remap(i, 0, steps - 1, 0f, 1f);
            source.volume = Mathf.Lerp(startingValue, targetVolume, progress);
            yield return new WaitForFixedUpdate();
        }
        if (targetVolume == 0f)
        {
            source.Stop();
            source.volume = startingValue;
        }
    }

    public static void AdjustAudioPitch(AudioSource source, float targetPitch, float duration, MonoBehaviour monoBehaviour)
    {
        monoBehaviour.StartCoroutine(AdjustAudioPitchCR(source, targetPitch, duration));
    }
    public static IEnumerator AdjustAudioPitchCR(AudioSource source, float targetPitch, float duration)
    {
        float startingValue = source.pitch;
        float smoothness = 0.01f;
        float progress = 0;
        float increment = smoothness / duration;

        while (progress < 1)
        {
            source.pitch = Mathf.Lerp(startingValue, targetPitch, progress);
            progress += increment;
            yield return new WaitForSeconds(smoothness);
        }
    }

    #endregion

    #region SceneManagement
    public static void TransitionToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public static void TransitionToScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public static void TransitionToScene(string sceneName, MonoBehaviour monoBehaviour, float delayBeforeTrans = 0f)
    {
        monoBehaviour.StartCoroutine(SceneTransition(sceneName, delayBeforeTrans));
    }
    public static void TransitionToScene(int sceneIndex, MonoBehaviour monoBehaviour, float delayBeforeTrans = 0f)
    {
        monoBehaviour.StartCoroutine(SceneTransition(sceneIndex, delayBeforeTrans));
    }
    private static IEnumerator SceneTransition(string sceneName, float delayBeforeTrans)
    {
        yield return new WaitForSeconds(delayBeforeTrans);
        SceneManager.LoadScene(sceneName);
    }
    private static IEnumerator SceneTransition(int sceneIndex, float delayBeforeTrans)
    {
        yield return new WaitForSeconds(delayBeforeTrans);
        SceneManager.LoadScene(sceneIndex);
    }
    #endregion

    #region GUI
    public static void SetSelectable(GameObject newSelectable)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(newSelectable);
    }
    #endregion

    #region Other
    // Checks wether to points are in the same position with set tolerance. 
    public static bool V3Equals(Vector3 pos1, Vector3 pos2, float tolerance = 0.01f)
    {
        if (Vector3.Distance(pos1, pos2) <= tolerance) return true;
        else return false;
    }

    public static bool V2Equals(Vector2 pos1, Vector2 pos2, float tolerance = 0.01f)
    {
        if (Vector2.Distance(pos1, pos2) <= tolerance) return true;
        else return false;
    }
	
    public static float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    #endregion
}
#endregion

#region CollectionExtensions
public static class CollectionExtentions
{
    // The method to retrieve all matching objects in a sorted or unsorted List<T>
    public static IEnumerable<T> GetAll<T>(this List<T> myList, T searchValue) => myList.Where(t => t.Equals(searchValue));
    // Count the number of times an item appears in this unsorted or sorted List<T>
    public static int CountAll<T>(this List<T> myList, T searchValue) => myList.GetAll(searchValue).Count();

    // The method to retrieve all matching objects in a sorted List<T>
    public static T[] BinarySearchGetAll<T>(this List<T> myList, T searchValue)
    {
        List<T> retObjs = new List<T>();
        int center = myList.BinarySearch(searchValue);
        if (center > 0)
        {
            retObjs.Add(myList[center]);
            int left = center;
            while (left > 0 && myList[left - 1].Equals(searchValue))
            {
                left -= 1;
                retObjs.Add(myList[left]);
            }
            int right = center;
            while (right < (myList.Count - 1) &&
            myList[right + 1].Equals(searchValue))
            {
                right += 1;
                retObjs.Add(myList[right]);
            }
        }
        return (retObjs.ToArray());
    }
    // Count the number of times an item appears in this sorted List<T>
    public static int BinarySearchCountAll<T>(this List<T> myList, T searchValue) => BinarySearchGetAll(myList, searchValue).Count();

}
#endregion

#region Random
static class RandomExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Shuffle<T>(this Container<T> container)
    {
        List<T> list = container.ToList();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
        container.FromList(list);
    }

    public static T RandomElement<T>(this IList<T> list)
    {
        return list[rng.Next(list.Count)];
    }
}
#endregion

#region CostumContainer
public class Container<T> : IEnumerable<T>
{
    public Container() { }
    private List<T> _internalList = new List<T>();
    // This iterator iterates over each element from first to last
    public IEnumerator<T> GetEnumerator() => _internalList.GetEnumerator();
    // This iterator iterates over each element from last to first
    public IEnumerable<T> GetReverseOrderEnumerator()
    {
        foreach (T item in ((IEnumerable<T>)_internalList).Reverse())
            yield return item;
    }
    // This iterator iterates over each element from first to last, stepping
    // over a predefined number of elements
    public IEnumerable<T> GetForwardStepEnumerator(int step)
    {
        foreach (T item in _internalList.EveryNthItem(step))
            yield return item;
    }
    // This iterator iterates over each element from last to first, stepping
    // over a predefined number of elements
    public IEnumerable<T> GetReverseStepEnumerator(int step)
    {
        foreach (T item in ((IEnumerable<T>)_internalList).Reverse().EveryNthItem(step))
            yield return item;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Clear()
    {
        _internalList.Clear();
    }
    public void Add(T item)
    {
        _internalList.Add(item);
    }

    public void AddRange(ICollection<T> collection)
    {
        _internalList.AddRange(collection);
    }
}

public static class ContainerExtension
{
    public static IEnumerable<T> EveryNthItem<T>(this IEnumerable<T> enumerable, int step)
    {
        int current = 0;
        foreach (T item in enumerable)
        {
            ++current;
            if (current % step == 0)
                yield return item;
        }
    }

    public static void FromList<T>(this Container<T> container, List<T> list)
    {
        container.Clear();
        foreach (T item in list) container.Add(item);
    }
}
#endregion
