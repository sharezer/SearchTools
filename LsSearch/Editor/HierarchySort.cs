using UnityEditor;
using UnityEngine;

public class 升序排列 : BaseHierarchySort
{
    public override GUIContent content
    {
        get { return new GUIContent("升序"); }
    }

    public override int Compare(GameObject lhs, GameObject rhs)
    {
        if (lhs == rhs) return 0;
        if (lhs == null) return -1;
        if (rhs == null) return 1;

        return EditorUtility.NaturalCompare(lhs.name, rhs.name);
    }
}

public class 降序排列 : BaseHierarchySort
{
    public override GUIContent content
    {
        get { return new GUIContent("降序"); }
    }

    public override int Compare(GameObject lhs, GameObject rhs)
    {
        if (lhs == rhs) return 0;
        if (lhs == null) return 1;
        if (rhs == null) return -1;

        return -EditorUtility.NaturalCompare(lhs.name, rhs.name);
    }
}