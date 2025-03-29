using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class SkillEvents
{
    public static event Action<Define.SkillTriggerType> onTriggerSkill;

    public static void TriggerSkills(Define.SkillTriggerType skillTriggerType)
    {
        if (onTriggerSkill != null)
        {
            onTriggerSkill(skillTriggerType);
        }
    }
}
