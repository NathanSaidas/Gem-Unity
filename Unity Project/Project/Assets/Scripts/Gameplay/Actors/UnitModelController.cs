using UnityEngine;


namespace Gem
{
    /// <summary>
    /// Model Controller
    /// - Has a reference to all the mesh renderers
    /// - Has a reference to all the mesh filters
    /// - Has a reference to all the skinned mesh renderers
    /// - Has a reference to all the important bone pivot transforms
    /// 
    /// - May Disable All Mesh Renderers or Mesh Filters
    /// - May Do Mesh Swap / Material Swap
    /// - Safe Access Properties of the Shaders
    /// 
    /// Shaders for Characters (All Include Fog Of War)
    /// - Diffuse (Default)
    /// - Diffuse + Outline (Character Selection (Mouse Hover))
    /// - Diffuse + Transparency + Dissolve (Special for Invisible Units)
    /// - Diffuse + Transparency + Dissolve + Outline (Special for Invisible Units and Selection (Mouse Hover))
    /// </summary>
    public class UnitModelController : MonoBehaviour
    {

        
    }

}

