using System.Linq;
using OpenTK.Graphics.OpenGL;
using Sledge.DataStructures.Geometric;
using Sledge.DataStructures.Models;
using Sledge.Graphics.Helpers;
using Sledge.Graphics.Renderables;

namespace Sledge.DataStructures.Rendering
{
    public class ModelRenderable : IRenderable
    {
        public Model Model { get; set; }
        public int CurrentAnimation { get; set; }
        public int CurrentFrame { get; set; }

        public bool DrawMesh { get; set; }
        public bool DrawWireframe { get; set; }
        public bool DrawBones { get; set; }
        public bool DrawPoints { get; set; }

        public ModelRenderable(Model model)
        {
            Model = model;
            CurrentAnimation = model.Animations.Count - 1;
            CurrentFrame = 0;
            DrawMesh = true;
        }

        public void Render(object sender)
        {
            var anim = CurrentAnimation < 0 ? null : Model.Animations[CurrentAnimation];
            var frame = anim != null ? anim.Frames[CurrentFrame] : null;
            var transforms = frame != null ? frame.GetBoneTransforms(Model.BonesTransformMesh, !Model.BonesTransformMesh) : Model.Bones.Select(x => x.Transform).ToList();
            var boneTransforms = frame != null && !Model.BonesTransformMesh ? frame.GetBoneTransforms(true, true) : transforms;

            if (DrawWireframe)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Disable(EnableCap.CullFace);
            }

            if (DrawMesh)
            {
                // Draw meshes
                GL.Color4(1f, 1f, 1f, 1f);
                GL.LineWidth(1);
                foreach (var mesh in Model.Meshes)
                {
                    var name = "Model/" + Model.Name + "/" + Model.Textures[mesh.SkinRef].Name;
                    var texture = TextureHelper.Create(name, Model.Textures[mesh.SkinRef].Image, false);
                    texture.Bind();
                    GL.Begin(BeginMode.Triangles);
                    foreach (var v in mesh.Vertices)
                    {
                        var transform = transforms[v.BoneWeightings.First().Bone.BoneIndex];
                        var c = v.Location * transform;
                        GL.TexCoord2(v.TextureU / texture.Width, v.TextureV / texture.Height);
                        GL.Vertex3(c.X, c.Y, c.Z);
                    }
                    GL.End();
                    TextureHelper.Unbind();
                    TextureHelper.Delete(name);
                }
            }

            if (DrawBones)
            {
                // Draw bones
                GL.Color3(1f, 0f, 0f);
                GL.LineWidth(2);
                foreach (var b in Model.Bones.Where(x => x.Parent != null))
                {
                    var transform = boneTransforms[b.BoneIndex];
                    var transform2 = boneTransforms[b.ParentIndex];
                    var offset = CoordinateF.Zero * transform;
                    var offset2 = CoordinateF.Zero * transform2;
                    GL.Begin(BeginMode.Lines);
                    GL.Vertex3(offset.X, offset.Y, offset.Z);
                    GL.Vertex3(offset2.X, offset2.Y, offset2.Z);
                    GL.End();
                }
                GL.LineWidth(1);
            }

            if (DrawPoints)
            {
                // Draw points
                GL.Color3(1f, 0f, 1f);
                GL.PointSize(5);
                foreach (var b in Model.Bones)
                {
                    var transform = boneTransforms[b.BoneIndex];
                    var offset = CoordinateF.Zero * transform;
                    GL.Begin(BeginMode.Points);
                    GL.Vertex3(offset.X, offset.Y, offset.Z);
                    GL.End();
                }
                GL.PointSize(1);
            }

            if (frame != null) CurrentFrame = (CurrentFrame + 1) % anim.Frames.Count;

            GL.Enable(EnableCap.CullFace);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }
}