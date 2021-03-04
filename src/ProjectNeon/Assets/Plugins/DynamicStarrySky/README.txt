Dynamic Starry Sky

Author: Jason Ederle at Funly
Website: http://funly.io
Contact: jason@funly.io
Description: Generate customizable starry skies.

Please feel free to contact me regarding any questions or issues!

Setup Instructions (Using Prefab):
- Drag a skybox prefab from DynamicStarrySky/Prefabs into your scene.
- Scale it to fit around your entire scene (make sure it's scaled uniformly on xyz to avoid distortion).
- That's it! Now play with the star shader material settings sliders.
- After making changes to the star "density", the material button "Rebuild Star System" will turn red. When the
  rebuild button is red, that means you need to click it to regenerate the star system. The star system data files
  are a bunch of precomputed values, which is what makes it possible for the shader to run super fast.

Setup Instructions (Using Unity Skybox):
- Drag any of the materials from DynamicStarrySky/Materials/StarrySky* into Unity's skybox material under the Lighting tab.
- That's it! Now play with the star material settings.
- After making changes to the star "density", the material button "Rebuild Star System" will turn red. When the
  rebuild button is red, that means you need to click it to regenerate the star system. The star system data files
  are a bunch of precomputed values, which is what makes it possible for the shader to run super fast.

Important Tips:
- To make stars glow, you'll need to add a bloom filter onto your camera. You can adjust Starry Sky "HDR Boost" value on the stars and moon to control glow amount. I
  recommend using Unity's Post Processing Effects, which have a great bloom filter: https://www.assetstore.unity3d.com/en/#!/content/83912
- You should disable any feature layers your not using to get a significant performance boost. You disable a layer by unchecking the layers checkbox on your skybox material.
- On Mobile devices (or low end hardware) you can further increase performance by using 1-2 star layers and NOT using a camera bloom filter.
- Custom star/moon textures need to be in additive colors for blending to work properly. Use black for transparent, and white for visible areas. See provided stars for examples.
- If stars overlap only 1 star will be chosen to render at that pixel fragment. The largest star layer number will be prefered over lower star layer numbers.
  Use the lower star layers for your less significant small stars, and the larger star layers for more important large stars.
- If you want to animate the moon at runtime, you need to use the component "MoonPositionHelper.cs". Attach it to any game object and give
  it a reference to your skybox material. This script will update the shader moon position, along with some other crucial precomputed hidden data.
