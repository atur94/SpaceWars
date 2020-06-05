using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class TargetSelectionSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D rayHit = Physics2D.Raycast(ray.origin, ray.direction);
            if (rayHit.collider != null)
            {
                Entities.ForEach((ref TargetSelector targetSelector) =>
                {
                    targetSelector.SecondaryTranslation = new Translation
                    {
                        Value = new float3(rayHit.point, 0f)
                    };
                });
            }


        }

    }
}
    

