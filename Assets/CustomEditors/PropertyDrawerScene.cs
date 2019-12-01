using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomEditors
{
    public class PropertyDrawerScene : MonoBehaviour
    {
        public Ingredient potionResult;
        public Ingredient[] potionIngredients;
    }

    public enum IngredientUnit
    {
        Cup,
        Bowl,
        Spoon,
        Piece
    }

    [Serializable] //这个特性表示该类可以被序列化，但是不加好像也没关系
    public class Ingredient
    {
        //需要重新设计属性面板的类
        public string name;
        public int amount;
        public IngredientUnit unit;
    }
}