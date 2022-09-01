


float4 MulQH( float4 q1, float4 q2 ){
			
			float4 q = {
				q1.x*q2.x - q1.y*q2.y - q1.z*q2.z - q1.w*q2.w,
				q1.x*q2.y + q1.y*q2.x + q1.z*q2.w - q1.w*q2.z,
				q1.x*q2.z - q1.y*q2.w + q1.z*q2.x + q1.w*q2.y,
				q1.x*q2.w + q1.y*q2.z - q1.z*q2.y + q1.w*q2.x
			};

			return q;
		}


		float3 rVQ(float3 v, float4 q){
		
			float4 vecQ = float4(0, v.xyz);
			float4 invQ = float4(q.x,-q.y,-q.z,-q.w);
			return MulQH(MulQH(q,vecQ),invQ).yzw;

		}


void CalcTensionMap_float(float4 texcoord1, float4 texcoord2, float4 texcoord3, float3 worldPos, float3 hBPos, float4 hBRot, float vThreshold, out float TensionMap){

#if SHADERGRAPH_PREVIEW
	texcoord1 = float4(0,0,0,0);
	texcoord2 = float4(0,0,0,0);
	texcoord3 = float4(0,0,0,0);
	TensionMap = 0;
#else
	
	float3 restPose = float3(texcoord1.z,texcoord1.w,texcoord2.x);
	float3 expPoseA = float3(texcoord2.y,texcoord2.z,texcoord2.w);
	float3 expPoseB = float3(texcoord3.x,texcoord3.y,texcoord3.z);
	float3 vertexPos = worldPos-hBPos.xyz;
			

	vertexPos = rVQ(vertexPos,hBRot);

	float distanceToRestPose = distance(vertexPos.xyz, restPose);
	float distanceToPoseA = distance(vertexPos.xyz, expPoseA)+vThreshold;
	float distanceToPoseB = distance(vertexPos.xyz, expPoseB)+vThreshold;
	float restPoseToPoseA = distance(restPose, expPoseA);
	float restPoseToPoseB = distance(restPose, expPoseB);

	float nRestToA = distanceToPoseA / restPoseToPoseA;
	float nRestToB = distanceToPoseB / restPoseToPoseB;

	TensionMap = saturate(1.2-min(nRestToA,nRestToB));

	TensionMap = saturate(pow(TensionMap*1.5,1.5));

#endif
	
}