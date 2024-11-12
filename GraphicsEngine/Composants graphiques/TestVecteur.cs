using System.Numerics; // Pour Vector3
using Xunit;

namespace VectorTests
{
    public class Vector3Tests
    {
        [Fact]
        public void TestNorm()
        {
            Vector3 vector = new Vector3(3f, 4f, 0f);
            float norm = vector.Length();
            Assert.Equal(5f, norm, 3); // Utilise Assert.Equal() avec une tolérance de précision
        }

        [Fact]
        public void TestSquareNorm()
        {
            Vector3 vector = new Vector3(3f, 4f, 0f);
            float squareNorm = vector.LengthSquared();
            Assert.Equal(25f, squareNorm, 3); // Utilise Assert.Equal() au lieu de AreEqual
        }

        [Fact]
        public void TestDotProduct()
        {
            Vector3 vector1 = new Vector3(1f, 2f, 3f);
            Vector3 vector2 = new Vector3(4f, 5f, 6f);
            float dotProduct = Vector3.Dot(vector1, vector2);
            Assert.Equal(32f, dotProduct, 3); // Test pour vérifier le produit scalaire
        }

        [Fact]
        public void TestNormalize()
        {
            Vector3 vector = new Vector3(3f, 4f, 0f);
            Vector3 normalizedVector = Vector3.Normalize(vector);
            Assert.Equal(1f, normalizedVector.Length(), 3); // Vérifie que la norme du vecteur est 1
        }

        [Fact]
        public void TestAddition()
        {
            Vector3 vector1 = new Vector3(1f, 2f, 3f);
            Vector3 vector2 = new Vector3(4f, 5f, 6f);
            Vector3 result = vector1 + vector2;
            Assert.Equal(new Vector3(5f, 7f, 9f), result); // Vérifie que l'addition des vecteurs est correcte
        }

        [Fact]
        public void TestSubtraction()
        {
            Vector3 vector1 = new Vector3(4f, 5f, 6f);
            Vector3 vector2 = new Vector3(1f, 2f, 3f);
            Vector3 result = vector1 - vector2;
            Assert.Equal(new Vector3(3f, 3f, 3f), result); // Vérifie que la soustraction des vecteurs est correcte
        }

        [Fact]
        public void TestScalarMultiplication()
        {
            Vector3 vector = new Vector3(1f, 2f, 3f);
            float scalar = 2f;
            Vector3 result = vector * scalar;
            Assert.Equal(new Vector3(2f, 4f, 6f), result); // Vérifie la multiplication par un scalaire
        }

        [Fact]
        public void TestScalarDivision()
        {
            Vector3 vector = new Vector3(2f, 4f, 6f);
            float scalar = 2f;
            Vector3 result = vector / scalar;
            Assert.Equal(new Vector3(1f, 2f, 3f), result); // Vérifie la division par un scalaire
        }

        [Fact]
        public void TestEquality()
        {
            Vector3 vector1 = new Vector3(1f, 2f, 3f);
            Vector3 vector2 = new Vector3(1f, 2f, 3f);
            Assert.True(vector1 == vector2); // Vérifie que les vecteurs sont égaux
        }

        [Fact]
        public void TestInequality()
        {
            Vector3 vector1 = new Vector3(1f, 2f, 3f);
            Vector3 vector2 = new Vector3(4f, 5f, 6f);
            Assert.True(vector1 != vector2); // Vérifie que les vecteurs sont différents
        }

        [Fact]
        public void TestToString()
        {
            Vector3 vector = new Vector3(1f, 2f, 3f);
            string vectorString = vector.ToString();
            Assert.Equal("<1, 2, 3>", vectorString); // Vérifie la conversion du vecteur en chaîne de caractères
        }
    }
}
