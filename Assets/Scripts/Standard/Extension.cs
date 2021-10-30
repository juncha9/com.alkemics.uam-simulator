using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using UnityEngine;

using System.Reflection;
using UnityEngine.UI;
using UAM;

/// <summary>
/// This extension is functional group that is our used frenquently
/// </summary>
public static partial class Extension
{

    #region [ Find Child ]

    public static Transform FindDeepChild(this Transform parent, string findName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == findName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    public static GameObject Find(this GameObject go, string findName, bool bSearchInChildren)
    {
        if (bSearchInChildren)
        {
            var transform = go.transform;
            var childCount = transform.childCount;
            //Debug.LogError( go.name + " ChildCount: " + childCount);
            for (int i = 0; i < childCount; ++i)
            {
                var child = transform.GetChild(i);
                if (child.gameObject.name == findName)
                    return child.gameObject;
                GameObject result = child.gameObject.Find(findName, bSearchInChildren);
                if (result != null) return result;
            }
            return null;
        }
        else
        {
            return GameObject.Find(findName);
        }
    }

    #endregion [ Find Child ]

    #region [ GameObject ]

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var result = gameObject.GetComponent<T>();
        if (result == default(T))
        {
            result = gameObject.AddComponent<T>();
        }
        return result;
    }

    public static bool HasComponent<T>(this GameObject gameObject) where T : Component
    {
        if(gameObject == null)
        {
            return false;
        }
        return gameObject.GetComponent<T>() != null;
    }

    #endregion [ GameObject ]

    #region [ Transform ]

    public static void AttachNext(this Transform thisTransform, Transform targetTransform)
    {
        if (thisTransform.parent != targetTransform.parent) return;
        thisTransform.SetSiblingIndex((targetTransform.GetSiblingIndex() + 1));
    }

    public static bool ContainPosition(this RectTransform rectTransform, Vector3 position)
    {
        bool isXInsided = false;
        bool isYInsided = false;

        float startX = rectTransform.position.x + rectTransform.rect.xMin;
        float endX = rectTransform.position.x + rectTransform.rect.xMax;

        isXInsided = position.x >= startX && position.x <= endX;

        float startY = rectTransform.position.y + rectTransform.rect.yMin;
        float endY = rectTransform.position.y + rectTransform.rect.yMax;

        isYInsided = position.y >= startY && position.y <= endY;

        return isXInsided && isYInsided;
    }

    #endregion

    #region [ RectTransform ]

    public static float GetLeftOnWorld(this RectTransform rectTransform)
    {
        return rectTransform.position.x + rectTransform.rect.xMin;
    }

    public static float GetRightOnWorld(this RectTransform rectTransform)
    {
        return rectTransform.position.x + rectTransform.rect.xMax;
    }

    public static float GetTopOnWorld(this RectTransform rectTransform)
    {
        return rectTransform.position.y + rectTransform.rect.yMax;
    }

    public static float GetBottomOnWorld(this RectTransform rectTransform)
    {
        return rectTransform.position.y + rectTransform.rect.yMin;
    }

    public static Vector3[] GetCorners(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return corners;
    }
    
    public static float MaxY(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[1].y;
    }

    public static float MinY(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[0].y;
    }

    public static float MaxX(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[2].x;
    }

    public static float MinX(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[0].x;
    }

    public static void StretchParent(this RectTransform rectTransform)
    {
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.anchoredPosition = new Vector2(0f, 0f);
        rectTransform.sizeDelta = new Vector2(0f, 0f);
    }

    #endregion

    #region [ IList ]

    public static string ToSplitText(this IList<string> list, string splitText = ";")
    {
        string text = "";
        for(int i = 0; i < list.Count; i++)
        {
            text += list[i];
            if (i < list.Count - 1)
            {
                //In
                text += splitText;
            }
            else
            {
                //Last
            }
        }
        return text;
    }


    public static IList<T> Swap<T>(this IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
        return list;
    }

    public static int SwapWith(this int input, ref int target)
    {
        var temp = input;
        input = target;
        target = temp;
        return input;
    }

    public static T Find<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
    {
        return enumerable.Where(x => predicate.Invoke(x)).FirstOrDefault();
    }

    public static List<T> FindAll<T>(this IEnumerable<T> enumerable, Predicate<T> predicate)
    {
        return enumerable.Where(x => predicate.Invoke(x)).ToList();
    }

    public static List<TOutput> CastAll<TOutput, T>(this IEnumerable<T> enumerable) where TOutput : T
    {
        return enumerable.Cast<TOutput>().ToList();
    }

    public static int RemoveAll<T>(this ICollection<T> collection, Predicate<T> predicate)
    {
        int count = 0;
        var removeList = collection.Where(x => predicate.Invoke(x)).ToArray();
        foreach (var item in removeList)
        {
            if (collection.Remove(item))
            {
                count++;
            }
        }
        return count;
    }

    //검증완료

    public static bool Contains<T>(this IEnumerable<T> list, Predicate<T> match)
    {
        return list.Any(x => match.Invoke(x));
    }

    //검증완료

    public static void Sort2<T>(this List<T> list, IList<T> target)
    {
        //list.OrderBy(a => target.FindIndex)



    }

    public static void Sort<T>(this List<T> list, IList<T> target)
    {
        List<T> _list = new List<T>(target);
        //지우기
        _list = _list.Intersect(list).ToList();
        //추가
        _list.AddRange(list.Where(x => _list.Contains(x) == false));

        list.Sort((a, b) =>
        {
            int indexA = _list.IndexOf(a);
            int indexB = _list.IndexOf(b);
            if (indexA >= 0 && indexB >= 0)
            {
                return indexA.CompareTo(indexB);
            }
            else if (indexA >= 0)
            {
                return -1;
            }
            else if (indexB >= 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });
    }

    public static void Sort<T>(this ObservableList<T> list, IList<T> target)
    {
        List<T> _list = new List<T>(target);
        //지우기
        _list = _list.Intersect(list).ToList();
        //추가
        _list.AddRange(list.Where(x => _list.Contains(x) == false));

        list.Sort((a, b) =>
        {
            int indexA = _list.IndexOf(a);
            int indexB = _list.IndexOf(b);
            if (indexA >= 0 && indexB >= 0)
            {
                return indexA.CompareTo(indexB);
            }
            else if (indexA >= 0)
            {
                return -1;
            }
            else if (indexB >= 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });
    }


    #endregion IList

    #region [ IDictionary ]

    public static string ToSplitText<KEY,VALUE>(this IDictionary<KEY,VALUE> dict, string splitText = ";")
    {

        int i = 0;
        StringBuilder str = new StringBuilder();
        foreach(var pair in dict)
        {
            str.Append($"{pair.Key}:{pair.Value}");
            if(i < dict.Count - 1)
            {
                str.Append(splitText);
            }
            else
            {

            }
        }
        return str.ToString();
    }



    public static void Log<T>(this ICollection<T> collection, string message)
    {
        string logText = "";
        logText += $"{message}\n";
        int index = 0;
        foreach (var item in collection)
        {
            if (item is UnityEngine.Object)
            {
                var converted = item as UnityEngine.Object;
                logText += $"[<color=yellow>{index}</color>] {converted.name}\n";
            }
            else
            {
                logText += $"[<color=yellow>{index}</color>] {item}\n";
            }
            index++;
        }
        Debug.Log(logText);
    }

    public static void Log<KEY, VALUE>(this IDictionary<KEY, VALUE> dict, string message)
    {
        string logText = "";
        logText += $"{message}\n";
        foreach (var key in dict.Keys)
        {
            if (dict[key] is UnityEngine.Object)
            {
                var converted = dict[key] as UnityEngine.Object;
                logText += $"[<color=yellow>{key}</color>] {converted.name}\n";
            }
            else
            {
                logText += $"[<color=yellow>{key}</color>] {dict[key]}\n";
            }
        }
        Debug.Log(logText);
    }

    public static void ChangeKey<KEY, VALUE>(this IDictionary<KEY, VALUE> thisDict, KEY oldKey, KEY newKey)
    {
        if (!thisDict.ContainsKey(oldKey))
        {
            //바뀌기 전 키를 가지고 있어야함
            throw new ArgumentException("Must contain old key");
        }
        if (thisDict.ContainsKey(newKey))
        {
            //바뀔 키를 가지고 있으면 안됨
            throw new ArgumentException("Must not contain new key");
        }
        VALUE temp = thisDict[oldKey];
        thisDict.Remove(oldKey);
        thisDict.Add(newKey, temp);
    }


    public static bool UnsortedEquals<VALUE>(this IEnumerable<VALUE> target, IEnumerable<VALUE> values)
    {
        if (values == null) return false;

        if(target.Except(values).Count() <= 0 && values.Except(target).Count() <= 0)
        {
            return true;
        }

        return false;
    }


    public static void MatchKeys<KEY, VALUE>(this IDictionary<KEY, VALUE> thisDict, ICollection<KEY> keys)
    {
        if (keys == null) return;
        /*
        Debug.Log("MatchStart");
        thisDict.Logging("Dict");
        matchKeys.Logging("MatchKeys");
        */

        var _keys = thisDict.Keys.ToArray();
        var removingKeys = _keys.Except(keys).ToArray();
        foreach (var removeKey in removingKeys)
        {
            thisDict.Remove(removeKey);
        }

        var addingKeys = keys.Except(_keys).ToArray();
        foreach (var addingKey in addingKeys)
        {
            thisDict.Add(addingKey, default(VALUE));
        }
    }

    public static void MatchKeysFill<KEY, VALUE>(this IDictionary<KEY, VALUE> thisDict, ICollection<KEY> matchKeys, VALUE _default)
    {
        if (matchKeys == null) return;
        /*
        Debug.Log("MatchStart");
        thisDict.Logging("Dict");
        matchKeys.Logging("MatchKeys");
        */

        var keys = thisDict.Keys.ToArray();
        var removingKeys = keys.Except(matchKeys).ToArray();
        foreach (var removeKey in removingKeys)
        {
            thisDict.Remove(removeKey);
        }
        var addingKeys = matchKeys.Except(keys).ToArray();
        foreach (var addingKey in addingKeys)
        {
            thisDict.Add(addingKey, _default);
        }
    }

    public static bool CompareByKey<KEY, VALUE>(this IDictionary<KEY, VALUE> dict, KEY key, IDictionary<KEY,VALUE> target)
    {
        if (dict == null) return false;
        if (dict.ContainsKey(key) == false) return false;
        if (target == null) return false;
        if (target.ContainsKey(key) == false) return false;

        VALUE thisValue = dict[key];
        VALUE targetValue = target[key];

        if (thisValue != null)
        {
            return thisValue.Equals(targetValue);
        }
        else
        {
            return targetValue.Equals(thisValue);
        }
    }

    #endregion [ IDictionary ]

    #region [ IList ]

    /// <summary>
    /// 해당 Index가 List개수의 범위에 포함되는지 여부를 체크하는 함수
    /// </summary>
    /// <param name="thisList"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool IsValidIndex<T>(this IList<T> thisList, int index)
    {
        int count = thisList.Count;
        if (count > 0)
        {
            if (index >= 0 && index < count)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    #endregion [ IList ]

    #region [ string ]

    public static bool HasWhitespace(this string text)
    {
        if(text == null)
        {
            return false;
        }
        else
        {
            return text.Contains(" ");
        }
    }

    public static List<string> SplitToList(this string text, char split)
    {
        var splited = text.Split(split);
        return splited.ToList();
    }


    /// <summary>
    /// Split and extract values from "[]"
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static ICollection<string> SplitSquareBrackets(this string text)
    {
        return Regex.Matches(text, @"\[([^]]*)\]")
        .Cast<Match>()
        .Select(x => x.Groups[1].Value)
        .ToList();
    }

    /// <summary>
    /// Split and extract values from "{}"
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static ICollection<string> SplitBraces(this string text)
    {
        return Regex.Matches(text, @"\{([^}]*)\}")
        .Cast<Match>()
        .Select(x => x.Groups[1].Value)
        .ToList();
    }

    /// <summary>
    /// Split and extract values from "()"
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static ICollection<string> SplitBrackets(this string text)
    {
        return Regex.Matches(text, @"\(([^)]*)\)")
        .Cast<Match>()
        .Select(x => x.Groups[1].Value)
        .ToList();
    }

    public static bool IsEmpty(this string s)
    {
        return string.IsNullOrWhiteSpace(s);
    }

    /// <summary>
    /// VB Left function
    /// </summary>
    /// <param name="stringparam"></param>
    /// <param name="numchars"></param>
    /// <returns>Left-most numchars characters</returns>
    public static string Left(this string stringparam, int numchars)
    {
        // Handle possible Null or numeric stringparam being passed
        stringparam += string.Empty;

        // Handle possible negative numchars being passed
        numchars = Math.Abs(numchars);

        // Validate numchars parameter
        if (numchars > stringparam.Length)
            numchars = stringparam.Length;

        return stringparam.Substring(0, numchars);
    }

    /// <summary>
    /// VB Right function
    /// </summary>
    /// <param name="stringparam"></param>
    /// <param name="numchars"></param>
    /// <returns>Right-most numchars characters</returns>
    public static string Right(this string stringparam, int numchars)
    {
        // Handle possible Null or numeric stringparam being passed
        stringparam += string.Empty;

        // Handle possible negative numchars being passed
        numchars = Math.Abs(numchars);

        // Validate numchars parameter
        if (numchars > stringparam.Length)
            numchars = stringparam.Length;

        return stringparam.Substring(stringparam.Length - numchars);
    }

    /// <summary>
    /// VB Mid function - to end of string
    /// </summary>
    /// <param name="stringparam"></param>
    /// <param name="startIndex">VB-Style startindex, 1st char startindex = 1</param>
    /// <returns>Balance of string beginning at startindex character</returns>
    public static string Mid(this string stringparam, int startindex)
    {
        // Handle possible Null or numeric stringparam being passed
        stringparam += string.Empty;

        // Handle possible negative startindex being passed
        startindex = Math.Abs(startindex);

        // Validate numchars parameter
        if (startindex > stringparam.Length)
            startindex = stringparam.Length;

        // C# strings are zero-based, convert passed startindex
        return stringparam.Substring(startindex - 1);
    }

    /// <summary>
    /// VB Mid function - for number of characters
    /// </summary>
    /// <param name="stringparam"></param>
    /// <param name="startIndex">VB-Style startindex, 1st char startindex = 1</param>
    /// <param name="numchars">number of characters to return</param>
    /// <returns>Balance of string beginning at startindex character</returns>
    public static string Mid(this string stringparam, int startindex, int numchars)
    {
        // Handle possible Null or numeric stringparam being passed
        stringparam += string.Empty;

        // Handle possible negative startindex being passed
        startindex = Math.Abs(startindex);

        // Handle possible negative numchars being passed
        numchars = Math.Abs(numchars);

        // Validate numchars parameter
        if (startindex > stringparam.Length)
            startindex = stringparam.Length;

        // C# strings are zero-based, convert passed startindex
        return stringparam.Substring(startindex - 1, numchars);
    }

    public static string ToTitleCase(this string str)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        return textInfo.ToTitleCase(str);
    }

    public static bool IsSnakeCase(this string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return false;
        }
        if(Regex.IsMatch(key, @"[a-zA-Z0-9_]", RegexOptions.Singleline))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static bool IsKeyString(this string str)
    {
        if (Regex.IsMatch(str, @"[^a-zA-Z0-9_]", RegexOptions.Singleline) == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region [ Animator ]

    /// <summary>
    /// Sets the weight of the layer at the given name
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="layerName">The layer name</param>
    /// <param name="weight">The new layer weight</param>
    public static void SetLayerWeight(this Animator animator, string layerName, float weight)
    {
        var layerIndex = animator.GetLayerIndex(layerName);
        if (layerIndex >= 0)
        {
            if (weight > 1)
            {
                weight = 1;
            }
            else if (weight < 0)
            {
                weight = 0;
            }
            animator.SetLayerWeight(layerIndex, weight);
        }
    }

    /// <summary>
    /// 애니메이터가 해당 파라미터를 가지고 있는지
    /// </summary>
    /// <param name="animator">검사할 애니메이터 컨트롤러</param>
    /// <param name="parameterName">파라미터 이름</param>
    /// <returns></returns>
    public static bool HasParameter(this Animator animator, string parameterName)
    {
        if (Application.isPlaying == false)
        {
            return false;
        }
        return animator.parameters.Contains(x => Equals(x.name, parameterName));
    }

    public static void TrySetBool(this Animator animator, string parameterName, bool value)
    {
        if (animator.HasParameter(parameterName) == false) return;
        animator.SetBool(parameterName, value);
    }

    public static void TrySetInteger(this Animator animator, string parameterName, int value)
    {
        if (animator.HasParameter(parameterName) == false) return;
        animator.SetInteger(parameterName, value);
    }

    public static void TrySetFloat(this Animator animator, string parameterName, float value)
    {
        if (animator.HasParameter(parameterName) == false) return;
        animator.SetFloat(parameterName, value);
    }

    public static void TrySetTrigger(this Animator animator, string parameterName)
    {
        if (animator.HasParameter(parameterName) == false) return;
        animator.SetTrigger(parameterName);
    }

    #endregion

    #region [ IEnumrable ]

    public static Type GetGenericType(this IEnumerable enumerable)
    {
        try
        {
            var cType = enumerable.GetType();
            if (cType.IsGenericType == false)
            {
                throw new Exception($"Enumerable is not generic collection type");
            }
            return cType.GetGenericArguments()[0];
        }
        catch
        {
            return null;
        }
    }

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable)
    {
        return new HashSet<T>(enumerable);
    }

    public static T FirstOrValue<T>(this IEnumerable<T> enumerable, T defaultValue) where T : class
    {
        try
        {
            return enumerable.First();
        }
        catch
        {
            return defaultValue;
        }
    }

    #endregion


    public static XElement ToXElement<T>(this object obj)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (TextWriter streamWriter = new StreamWriter(memoryStream))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(streamWriter, obj);
                string xValue = Encoding.ASCII.GetString(memoryStream.ToArray());
                return XElement.Parse(xValue, LoadOptions.SetBaseUri);
            }
        }
    }

    public static T FromXElement<T>(this XElement xElement)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        return (T)xmlSerializer.Deserialize(xElement.CreateReader());
    }


    #region [ Type ]

    public static List<FieldInfo> GetConstants(this Type type)
    {
        var fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
        return fieldInfos.Where(x => x.IsLiteral && x.IsInitOnly == false).ToList();
    }

    #endregion


    #region [ Color ]

    public static void ChangeColor(this Graphic graphic, float? r = null, float? g = null, float? b = null, float? a = null)
    {
        var _color = graphic.color;
        if(r.HasValue == true)
        {
            _color.r = r.Value;
        }
        if (g.HasValue == true)
        {
            _color.g = g.Value;
        }
        if (b.HasValue == true)
        {
            _color.b = b.Value;
        }
        if (a.HasValue == true)
        {
            _color.a = a.Value;
        }
        graphic.color = _color;
    }


    #endregion

}