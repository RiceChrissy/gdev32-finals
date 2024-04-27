#create a text file that creates c++ code which creates arrays of indices for each texture.
# f {vertex index}/{vertex texture coordinate index}/{vertex normal index}
# don't forget to triangulate the model before saving as obj.
v = [] #list of sets of coordinates of shape vertices
vn = [] #list of sets of vertex normals
vt = [] #list of sets of texture coordinates
f = [] #list of faces
finalVertices = []
finalIndices = [] # was actually not needed in the end, but left it here anyway
tangentSet = []
textureNames = [] #chris, gets names of unique textures
textures = [] #chris, gets faces of unique textures
def vec3Subtract(a, b):
    ax = a[0]
    ay = a[1]
    az = a[2]

    bx = b[0]
    by = b[1]
    bz = b[2]

    final = []
    final.append(ax - bx)
    final.append(ay - by)
    final.append(az - bz)

    return final

def vec2Subtract(a, b):
    ax = a[0]
    ay = a[1]

    bx = b[0]
    by = b[1]

    final = []
    final.append(ax - bx)
    final.append(ay - by)

    return final

def parseVertices(vertices):
    objfile = open(vertices)
    name = ""

    for line in objfile:

        if(line[0] == "v" and line[1] == " "):
            x = line.split()
            if("v" in x):
                x.remove("v")
            y = [float(vertices) for vertices in x]
            v.append(y)
        elif(line[0] == "v" and line[1] == "n"):
            x = line.split()
            if("vn" in x):
                x.remove("vn")
            y = [float(vertNormals) for vertNormals in x]
            vn.append(y)
        elif(line[0] == "v" and line[1] == "t"): # texture coordinates
            x = line.split()
            if("vt" in x):
                x.remove("vt")
            y = [float(texCoords) for texCoords in x]
            vt.append(y)
        #Listing all the materials used
        elif(line.startswith("usemtl") and line.startswith("usemtl tools/toolsnodraw") == False):
            name = line.split("/")[-1].replace("\n","")
            if name not in textureNames:
                textureNames.append(name)
        elif(line[0] == "f" and line[1] == " "): # change to support the faces which include the textures
            lineReplaced = line.replace('/',',')
            lineReplaced = lineReplaced.replace('f ','') #chris
            lineReplaced = lineReplaced.replace(' ',',') #chris
            lineReplaced = lineReplaced.replace('\n',' ') #chris
            if (name in textureNames):
                if (len(textures) <= textureNames.index(name)):
                    textures.append(lineReplaced)
                elif (len(textures) > textureNames.index(name)):
                    textures[textureNames.index(name)] = textures[textureNames.index(name)]+lineReplaced

    objfile.close()
    # Each element of textures is a unique texture
    # Each textureFaces index is a list of vertices for one instance of that unique texture
    for textureFaces in textures:

        #textures[i].split() = ['7,5,2,6,6,2,5,7,2', '5,8,3,2,9,3,1,10,3', '4,11,4,7,12,4,3,13,4']
        texList = textureFaces.split()

        #codeblock will make textureFaces = [[7,5,2,6,6,2,5,7,2], [5,8,3,2,9,3,1,10,3], [4,11,4,7,12,4,3,13,4]]
        tempTexFace = []
        for textLine in texList:
            x = textLine.split(',')
            for i in range(len(x)):
                x[i] = int(x[i])
            tempTexFace.append(x)
        textures[textures.index(textureFaces)] = tempTexFace

        #codeblock will make textureFaces = [7,5,2,6,6,2,5,7,2, 5,8,3,2,9,3,1,10,3, 4,11,4,7,12,4,3,13,4]
        # tempTexFace = []
        # for textLine in texList:
        #     x = textLine.split(',')
        #     for i in range(len(x)):
        #         x[i] = int(x[i])
        #     tempTexFace+=x
        # textures[textures.index(textureFaces)] = tempTexFace

        textureFaces = tempTexFace

        #textLine is an element that is a 9-size list.
        for faces in textureFaces:
            
            # vertex / texture / normal
            fv1_index = faces[0] # first point
            fvt1_index = faces[1]
            fvn1_index = faces[2]

            fv2_index = faces[3] # second point
            fvt2_index = faces[4]
            fvn2_index = faces[5]

            fv3_index = faces[6] # third point
            fvt3_index = faces[7]
            fvn3_index = faces[8]
            # forms an entire triangle

            edge1 = vec3Subtract(v[fv2_index-1],v[fv1_index-1])
            edge2 = vec3Subtract(v[fv3_index-1],v[fv1_index-1])
            deltaUV1 = vec2Subtract(vt[fvt2_index-1],vt[fvt1_index-1])
            deltaUV2 = vec2Subtract(vt[fvt3_index-1],vt[fvt1_index-1])

            # floatf = 1/ (deltaUV1[0] * deltaUV2[1] - deltaUV2[0] * deltaUV1[1])
            if (deltaUV1[0] * deltaUV2[1] - deltaUV2[0] * deltaUV1[1] == 0):
                floatf = 1.0
            else:   
                floatf = 1.0 / (deltaUV1[0] * deltaUV2[1] - deltaUV2[0] * deltaUV1[1])
            
            tanx = floatf * (deltaUV2[1] * edge1[0] - deltaUV1[1] * edge2[0]) # f * (deltaUV2.y * edge1.x - deltaUV1.y * edge2.x);
            tany = floatf * (deltaUV2[1] * edge1[1] - deltaUV1[1] * edge2[1])
            tanz = floatf * (deltaUV2[1] * edge1[2] - deltaUV1[1] * edge2[2])
            
            tangentSet.append(tanx)
            tangentSet.append(tany)
            tangentSet.append(tanz)

            # pair 1
            for coordinates in v[fv1_index-1]:
                finalVertices.append(coordinates)
            for coordinates in vn[fvn1_index-1]:
                finalVertices.append(coordinates)
            finalVertices.append(tanx)
            finalVertices.append(tany)
            finalVertices.append(tanz)
            for coordinates in vt[fvt1_index-1]:
                finalVertices.append(coordinates)

            # pair 2
            for coordinates in v[fv2_index-1]:
                finalVertices.append(coordinates)
            for coordinates in vn[fvn2_index-1]:
                finalVertices.append(coordinates)
            finalVertices.append(tanx)
            finalVertices.append(tany)
            finalVertices.append(tanz)
            for coordinates in vt[fvt2_index-1]:
                finalVertices.append(coordinates)

            # pair 3
            for coordinates in v[fv3_index-1]:
                finalVertices.append(coordinates)
            for coordinates in vn[fvn3_index-1]:
                finalVertices.append(coordinates)
            finalVertices.append(tanx)
            finalVertices.append(tany)
            finalVertices.append(tanz)
            for coordinates in vt[fvt3_index-1]:
                finalVertices.append(coordinates)

        finalIndices.append(finalVertices[:])
        finalVertices.clear()
        with open('textureTest.txt', 'w') as test:
            test.write("finalIndices Size: " + str(len(finalIndices)))
            test.write("\n")
            test.write("textureNames Size: " + str(len(textureNames)))
            test.write("\n")
            test.write("textureFaces Size: " + str(len(textures)))
            test.write("\n")
            test.write("\n")

            for e in range(len(finalIndices)):
                test.write('float ' + textureNames[e] + '[] = {')
                test.write("\n")
                count = 0
                for i in range(len(finalIndices[e])):
                    test.write(str(round(finalIndices[e][i], 5))+'f')
                    test.write(", ")
                    count+=1
                    if(count == 3 or count == 6 or count == 9):
                        test.write("\t\t")
                    if(count == 11):
                        test.write("\n")
                        count=0
                # test.write(str(finalIndices[e]))
                # test.write(str(round(textures[e], 5)))
                test.write("\n")
                test.write("};")
                test.write("\n")
        

parseVertices("Construct_half_scaled_OGroup.obj")

#     with open('Construct_Half_Vertices.txt','w') as newVerts:
#         count = 0
#         for e in finalVertices:
#             newVerts.write(str(round(e, 5)) + 'f')
#             count+=1
#             newVerts.write(', ')
#             if(count == 3 or count == 6 or count == 9):
#                 newVerts.write('\t\t')
#             if(count == 11):
#                 newVerts.write('\n')
#                 count=0
    #textureFaces[0] = 7,5,2,6,6,2,5,7,2 5,8,3,2,9,3,1,10,3 4,11,4,7,12,4,3,13,4
    #textureFaces[0].split() = ['7,5,2,6,6,2,5,7,2', '5,8,3,2,9,3,1,10,3', '4,11,4,7,12,4,3,13,4']
