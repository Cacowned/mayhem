using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using MayhemWebCamWrapper;

//this cover flow control us based on the tutorial at http://d3dal3.blogspot.com/2009/04/wpf-cover-flow-tutorial-part-7-source.html
namespace MayhemVisionModules.Wpf
{
    class CoverFlowElement : ModelVisual3D
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CoverFlowElement(ImageRenderer renderer, int coverPos, int currentPos, ModelVisual3D model)
        {
            pos = coverPos;
            visualModel = model;

            imageSource = renderer;
            modelGroup = new Model3DGroup();
            modelGroup.Children.Add(new GeometryModel3D(Tessellate(), LoadImage(imageSource.Source)));
            modelGroup.Children.Add(new GeometryModel3D(TessellateMirror(), LoadImageMirror(imageSource.Source)));

            rotation = new AxisAngleRotation3D(new Vector3D(0, 1, 0), RotationAngle(currentPos));
            translation = new TranslateTransform3D(TranslationX(currentPos), 0, TranslationZ(currentPos));
            var transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(new RotateTransform3D(rotation));
            transformGroup.Children.Add(translation);
            modelGroup.Transform = transformGroup;

            Content = modelGroup;
            visualModel.Children.Add(this);
        }

        public void Destroy()
        {
            visualModel.Children.Remove(this);
        }

        public void Animate(int index, bool animate)
        {
            if (animate || rotation.HasAnimatedProperties)
            {
                var rotateAnimation = new DoubleAnimation(RotationAngle(index), AnimationDuration);
                var xAnimation = new DoubleAnimation(TranslationX(index), AnimationDuration);
                var zAnimation = new DoubleAnimation(TranslationZ(index), AnimationDuration);
                rotation.BeginAnimation(AxisAngleRotation3D.AngleProperty, rotateAnimation);
                translation.BeginAnimation(TranslateTransform3D.OffsetXProperty, xAnimation);
                translation.BeginAnimation(TranslateTransform3D.OffsetZProperty, zAnimation);
            }
            else
            {
                rotation.Angle = RotationAngle(index);
                translation.OffsetX = TranslationX(index);
                translation.OffsetZ = TranslationZ(index);
            }
        }

        /// <summary>
        /// Generates the rectangle used for rendering
        /// </summary>
        private Geometry3D Tessellate()
        {
            double dx = RectangleDx();
            double dy = RectangleDy();
            var p0 = new Point3D(-1 + dx, -1 + dy, 0);
            var p1 = new Point3D(1 - dx, -1 + dy, 0);
            var p2 = new Point3D(1 - dx, 1 - dy, 0);
            var p3 = new Point3D(-1 + dx, 1 - dy, 0);
            var q0 = new Point(0, 0);
            var q1 = new Point(1, 0);
            var q2 = new Point(1, 1);
            var q3 = new Point(0, 1);
            return BuildMesh(p0, p1, p2, p3, q0, q1, q2, q3);
        }

        /// <summary>
        /// Generates the mirror image
        /// </summary>
        private Geometry3D TessellateMirror()
        {
            double dx = RectangleDx();
            double dy = RectangleDy();
            var p0 = new Point3D(-1 + dx, -3 + 3 * dy, 0);
            var p1 = new Point3D(1 - dx, -3 + 3 * dy, 0);
            var p2 = new Point3D(1 - dx, -1 + dy, 0);
            var p3 = new Point3D(-1 + dx, -1 + dy, 0);
            var q0 = new Point(0, 1);
            var q1 = new Point(1, 1);
            var q2 = new Point(1, 0);
            var q3 = new Point(0, 0);
            return BuildMesh(p0, p1, p2, p3, q0, q1, q2, q3);
        }

        /// <summary>
        /// Constructs a mesh representing a square
        /// </summary>
        private static MeshGeometry3D BuildMesh(Point3D p0, Point3D p1, Point3D p2, Point3D p3,
                                                Point q0, Point q1, Point q2, Point q3)
        {
            var mesh = new MeshGeometry3D();
            mesh.Positions.Add(p0);
            mesh.Positions.Add(p1);
            mesh.Positions.Add(p2);
            mesh.Positions.Add(p3);

            var normal = CalculateNormal(p0, p1, p2);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.TextureCoordinates.Add(q3);
            mesh.TextureCoordinates.Add(q2);
            mesh.TextureCoordinates.Add(q1);

            normal = CalculateNormal(p2, p3, p0);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.Normals.Add(normal);
            mesh.TextureCoordinates.Add(q0);
            mesh.TextureCoordinates.Add(q1);
            mesh.TextureCoordinates.Add(q2);

            mesh.Freeze();
            return mesh;
        }

        /// <summary>
        /// Compute the normal of a triangle
        /// </summary>
        private static Vector3D CalculateNormal(Point3D p0, Point3D p1, Point3D p2)
        {
            var v0 = new Vector3D(p1.X - p0.X, p1.Y - p0.Y, p1.Z - p0.Z);
            var v1 = new Vector3D(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return Vector3D.CrossProduct(v0, v1);
        }

        /// <summary>
        /// Image source loading for the main view and the mirror view
        /// </summary>
        private static Material LoadImage(ImageSource imSrc)
        {
            return new DiffuseMaterial(new ImageBrush(imSrc));
        }
        private static Material LoadImageMirror(ImageSource imSrc)
        {
            var image = new Image { Source = imSrc };
            Color color = Color.FromArgb(127, 255, 255, 255);
            image.OpacityMask = new LinearGradientBrush(color, color, 90.0);
            var brush = new VisualBrush(image);
            return new DiffuseMaterial(brush);
        }

        /// <summary>
        /// rendering dimensions
        /// </summary>
        private double RectangleDx()
        {
            if (imageSource.Width > imageSource.Height)
                return 0;
            return 1 - imageSource.Width / imageSource.Height;
        }

        private double RectangleDy()
        {
            if (imageSource.Width > imageSource.Height)
                return 1 - imageSource.Height / imageSource.Width;
            return 0;
        }

        /// <summary>
        /// Geometry transformation
        /// </summary>
        private double RotationAngle(int index)
        {
            return Math.Sign(pos - index) * -90;
        }
        private double TranslationX(int index)
        {
            return pos * CoverStep + Math.Sign(pos - index) * 1.5;
        }
        private double TranslationZ(int index)
        {
            return pos == index ? 1 : 0;
        }

        #region Constants
        public const double CoverStep = .2;
        public static readonly TimeSpan AnimationDuration = TimeSpan.FromMilliseconds(400);
        #endregion
        
        #region Fields
        private readonly ModelVisual3D visualModel;
        private readonly ImageRenderer imageSource;
        private readonly Model3DGroup modelGroup;
        private readonly AxisAngleRotation3D rotation;
        private readonly TranslateTransform3D translation;
        private readonly int pos;
        #endregion
    }
}
