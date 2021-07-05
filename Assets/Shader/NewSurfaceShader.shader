Shader "Custom/ObjectTriplanar"
{
    Properties
    {
        _PixelsPerUnit("Pixels Per Unit", Float) = 128.0
        _Color("Color", Color) = (1, 1, 1, 1)
        _MainTex("Albedo", 2D) = "white" { }
        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _BumpScale("Bump Scale", Float) = 1.0
        [Normal][NoScaleOffset] _BumpMap("Normal", 2D) = "bump" { }
        [NoScaleOffset] _OcclusionMap("Occlusion", 2D) = "white" { }
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" "DisableBatching" = "True" }

        CGPROGRAM

        // OpenGL ES�ɂ͔�Ή�...OpenGL ES�Ή��ɂ������ꍇ�A�񐳕��s����g�p���Ă���ӏ�������������K�v������
        #pragma exclude_renderers gles
        #pragma surface surf Standard vertex:vert fullforwardshadows addshadow
        #pragma target 3.5

        float _PixelsPerUnit;
        fixed4 _Color;
        sampler2D _MainTex;
        float4 _MainTex_ST;
        float4 _MainTex_TexelSize;
        half _Glossiness;
        half _Metallic;
        half _BumpScale;
        sampler2D _BumpMap;
        sampler2D _OcclusionMap;

        struct Input
        {
            float3 objectUV;
            float3 objectNormal;
            float4 objectTangent;
        };

        // �J�b�g�I�t�����̍�����Z�o
        // �J�b�g�I�t��0.57���炢������ƁA�R�[�i�[�������������Ă��܂��̂Œ���
        float3 cutoffBlending(float3 normalAbs, float cutoff)
        {
            float3 factor = max(normalAbs - cutoff, 0.0001);
            return factor / dot(factor, (float3)1.0);
        }

        // �p������̍�����Z�o
        float3 powerBlending(float3 normalAbs, float exponent)
        {
            float3 factor = pow(normalAbs, exponent);
            return factor / dot(factor, (float3)1.0);
        }

        // Y-XZ��Ώ̕����̍�����Z�o
        // �l�H���̃e�N�X�`���ɂ͂��ꂪ���ʓI��?
        float3 asymmetricBlending(float3 normalAbs, float exponent, float capCos, float borderHalfWidth)
        {
            float normalAbsXZMax = max(normalAbs.x, normalAbs.z);
            float2 factorXZ = pow(normalAbsXZMax > 0.0 ? normalAbs.xz / normalAbsXZMax : (float2)1.0, exponent);
            factorXZ /= dot(factorXZ, (float2)1.0);
             float factorY = smoothstep(capCos - borderHalfWidth, capCos + borderHalfWidth, normalAbs.y);
            float factorH = 1.0 - factorY;
            float3 factor = float3(factorXZ.x * factorH, factorY, factorXZ.y * factorH);
            return factor;
        }

        // ����xyz�A����w�ŕ\��������]�N�H�[�^�j�I�����s��`���ɂ���
        float3x3 rotationFromQuaternion(float4 q)
        {
            float3 nA = q.xyz * 2.0;
            float3 nB = q.xyz * nA;
            float3 nC = q.xxy * nA.yzz;
            float3 nD = q.www * nA;
            float3 nE = 1.0 - (nB.yzx + nB.zxy);
            float3 nF = nC + nD.zyx;
            float3 nG = nC - nD.zyx;
            return float3x3(
                nE.x, nG.x, nF.y,
                nF.x, nE.y, nG.z,
                nG.y, nF.z, nE.z);
        }

        // from����to�ւ̍ŒZ��]�s����쐬����
        float3x3 fromToRotation(float3 from, float3 to)
        {
            float3 axisU = cross(from, to);
            float axisULength = length(axisU);
            float clampedAxisULength = max(axisULength, 0.001);
            float angleIsValid = sign(axisULength);
            float cosTheta = dot(from, to);
            float3 axis = angleIsValid > 0.0 ? axisU / clampedAxisULength : float3(1.0, 0.0, 0.0);
            float2 cosThetaHSinThetaH = sqrt(((float2)1.0 + float2(1.0, -1.0) * cosTheta) * 0.5);
            return rotationFromQuaternion(float4(axis * cosThetaHSinThetaH.y, cosThetaHSinThetaH.x));
        }

        void vert(inout appdata_full v, out Input IN)
        {
            UNITY_INITIALIZE_OUTPUT(Input, IN);

            // 3�ʂ�UV���W
            // �I�u�W�F�N�g���W���x�[�X�Ƃ��A����Ƀ��[���h�X�P�[������l��������
            // ���[���h��Ԃɂ�����e�N�X�`���̃X�P�[�������ɂȂ�悤�ɂ���
            IN.objectUV = v.vertex.xyz * float3(
                length(unity_ObjectToWorld._m00_m10_m20),
                length(unity_ObjectToWorld._m01_m11_m21),
                length(unity_ObjectToWorld._m02_m12_m22));

            // �I�u�W�F�N�g�@��
            IN.objectNormal = v.normal;

            // �I�u�W�F�N�g�ڐ�
            IN.objectTangent = v.tangent;
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // �I�u�W�F�N�g���->Unity�ڋ�Ԃւ̕ϊ����쐬
            // �ڐ���w�ɂ͏]�@���̔��]�̗L�����i�[����Ă���̂ŁA������]�@���ɂ����Ă���
            float3 objectNormal = normalize(IN.objectNormal);
            float3 objectTangent = normalize(IN.objectTangent.xyz);
            float3 objectBinormal = cross(objectNormal, objectTangent) * IN.objectTangent.w;
            float3x3 objectToTangent = float3x3(objectTangent, objectBinormal, objectNormal);

            // �I�u�W�F�N�g�@���̐����̐�Βl�����߂�...3�����̃u�����h��Z�o�Ɏg��
            float3 objectNormalAbs = abs(objectNormal);

            // �I�u�W�F�N�g�@���̐����̕��������߂�...UV�E�ڋ�ԕ␳�Ɏg��
            float3 s = sign(objectNormal);

            // 3������UV���W���쐬
            // �T�C�R�����\���ɐ؂�J���W�J���C���[�W�������߁Akeijiro��������ƈقȂ�3�����̎�����Ώ̂ɂȂ��Ă���
            //
            //    +Y
            // +Z -X -Z +X
            //    -Y
            //
            float2 uvMultiplier = _PixelsPerUnit * _MainTex_TexelSize.xy * _MainTex_ST.xy;
            float2 objectUVX = _MainTex_ST.zw + uvMultiplier * IN.objectUV.zy * float2(s.x, 1.0);
            float2 objectUVY = _MainTex_ST.zw + uvMultiplier * IN.objectUV.zx * float2(-1.0, s.y);
            float2 objectUVZ = _MainTex_ST.zw + uvMultiplier * IN.objectUV.xy * float2(-s.z, 1.0);

            // 3�����̕��ʐڋ��->�I�u�W�F�N�g��Ԃւ̕ϊ����쐬
            // ���������͂�3�����̐ڋ�Ԃ���Ώ̂ɂȂ��Ă���
            // �ڋ�Ԃ̊��͉E��n�ɂȂ��Ă���_�ɒ���
            // �e�N�X�`���Ԃ�������E�A�΂��������A���������O���w��
            float3 tPXNormal = float3(s.x, 0.0, 0.0);
            float3 tPYNormal = float3(0.0, s.y, 0.0);
            float3 tPZNormal = float3(0.0, 0.0, s.z);
            float3 tPXTangent = float3(0.0, 0.0, s.x);
            float3 tPYTangent = float3(0.0, 0.0, -1.0);
            float3 tPZTangent = float3(-s.z, 0.0, 0.0);
            float3 tPXBinormal = float3(0.0, 1.0, 0.0);
            float3 tPYBinormal = float3(s.y, 0.0, 0.0);
            float3 tPZBinormal = float3(0.0, 1.0, 0.0);
            float3x3 tPXToObject = transpose(float3x3(tPXTangent, tPXBinormal, tPXNormal));
            float3x3 tPYToObject = transpose(float3x3(tPYTangent, tPYBinormal, tPYNormal));
            float3x3 tPZToObject = transpose(float3x3(tPZTangent, tPZBinormal, tPZNormal));

            // 3�����̖@��->�I�u�W�F�N�g�@���ւ̉�]�����߁A���ʐڋ��->Unity�ڋ�Ԃւ̍����ϊ����쐬
            float3x3 tPXToTangent = mul(objectToTangent, mul(fromToRotation(tPXNormal, objectNormal), tPXToObject));
            float3x3 tPYToTangent = mul(objectToTangent, mul(fromToRotation(tPYNormal, objectNormal), tPYToObject));
            float3x3 tPZToTangent = mul(objectToTangent, mul(fromToRotation(tPZNormal, objectNormal), tPZToObject));

            // 3�����̖@�����e�N�X�`������擾�A���ł�Unity�ڋ�ԏ�̃x�N�g���ɕϊ�
            float3x3 normals = transpose(float3x3(
                mul(tPXToTangent, UnpackScaleNormal(tex2D(_BumpMap, objectUVX), _BumpScale)),
                mul(tPYToTangent, UnpackScaleNormal(tex2D(_BumpMap, objectUVY), _BumpScale)),
                mul(tPZToTangent, UnpackScaleNormal(tex2D(_BumpMap, objectUVZ), _BumpScale))));

            // 3�����̃A���x�h�E�Օ������e�N�X�`������擾
            float4x3 albedoOcclusions = transpose(float3x4(
                tex2D(_MainTex, objectUVX).rgb, tex2D(_OcclusionMap, objectUVX).a,
                tex2D(_MainTex, objectUVY).rgb, tex2D(_OcclusionMap, objectUVY).a,
                tex2D(_MainTex, objectUVZ).rgb, tex2D(_OcclusionMap, objectUVZ).a));

            // 3�����̍�������Z�o
            // Ben Golus����̕��@���Q�l�ɂ����AY-XZ��Ώ̍������̗p
            float3 factor = asymmetricBlending(objectNormalAbs, 64.0, 0.5, 1.0 / 64.0);
            // �p�捬���̏ꍇ�͉��L
            //float3 factor = powerBlending(objectNormalAbs, 128.0);

            // �A���x�h�E�Օ���������
            float4 albedoOcclusion = mul(albedoOcclusions, factor) * float4(_Color.rgb, 1.0);

            // �@��������
            float3 normal = normalize(mul(normals, factor));

            //#define DEBUG_TANGENT_NORMAL
            //#define DEBUG_OBJECT_NORMAL
            //#define DEBUG_WORLD_NORMAL
            //#define DEBUG_ALBEDO
            //#define DEBUG_OCCLUSION

            o.Albedo = albedoOcclusion.rgb;
            o.Normal = float3(0.0, 0.0, 1.0);
            o.Metallic = 0.0;
            o.Smoothness = 0.0;
            o.Occlusion = albedoOcclusion.a;
            #if defined(DEBUG_TANGENT_NORMAL)
                o.Albedo = normal * 0.5 + 0.5;
            #elif defined(DEBUG_OBJECT_NORMAL)
                o.Albedo = mul(transpose(objectToTangent), normal) * 0.5 + 0.5;
            #elif defined(DEBUG_WORLD_NORMAL)
                o.Albedo = UnityObjectToWorldNormal(mul(transpose(objectToTangent), normal)) * 0.5 + 0.5;
            #elif defined(DEBUG_ALBEDO)
            #elif defined(DEBUG_OCCLUSION)
                o.Albedo = albedoOcclusion.a;
            #else
                o.Normal = normal;
                o.Metallic = _Metallic;
                o.Smoothness = _Glossiness;
            #endif
        }
        ENDCG
    }
        FallBack Off
}