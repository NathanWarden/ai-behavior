using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AiBehaviorVariable
{
	public string name;
	public VarType varType;
	public string value;

	public enum VarType
	{
		Float,
		Integer,
		Boolean
	}


	public AiBehaviorVariable(string name, VarType type, string value)
	{
		this.name = name;
		this.varType = type;
		this.value = value;
	}

	public AiBehaviorVariable()
	{
		this.name = "MyVar";
		this.varType = VarType.Boolean;
		this.value = "True";
	}


	public bool IsFloat()
	{
		return varType == VarType.Float;
	}

	public bool IsInteger()
	{
		return varType == VarType.Integer;
	}

	public bool IsBoolean()
	{
		return varType == VarType.Boolean;
	}

	public float GetFloatValue()
	{
		float floatValue = 0.0f;
		float.TryParse(value, out floatValue);
		return floatValue;
	}

	public int GetIntValue()
	{
		int intValue = 0;
		int.TryParse(value, out intValue);
		return intValue;
	}

	public bool GetBoolValue()
	{
		return (value == "true" || value == "True");
	}
}
	