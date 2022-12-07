#include <jni.h>
#include <android/log.h>
#include <GLES2/gl2.h>
#include <stdlib.h>
#include <sys/time.h>
#include "imgui/imgui.h"

#define LOGI(...) ((void)__android_log_print(ANDROID_LOG_INFO,"ImguiTest", __VA_ARGS__))
#define  LOGE(...)  __android_log_print(ANDROID_LOG_ERROR,"ImguiTest",__VA_ARGS__)

#include "imgui_impl_android.h"

void renderFrame();
bool setupGraphics(int width, int height);

int startTime, screenWidth, screenHeight;
ImVec4 imClearColor;
bool showTestWindow;
bool showAnotherWindow;

GLuint gProgram;
GLuint gvPositionHandle;
GLuint vbo;
const GLfloat gTriangleVertices[] = { 0.0f, 1.0f, -1.0f, -1.0f,
        1.0f, -1.0f };
		
long long currentTimeInMilliseconds()
{
    struct timeval tv;
    gettimeofday(&tv, NULL);
    return ((tv.tv_sec * 1000) + (tv.tv_usec / 1000));
}

extern "C"
void
Java_com_imgui_android_exampleapp_RendererWrapper_nativeOnSurfaceCreated(
        JNIEnv *env,
        jobject /* this */) {
    //LOGI("%s", "RendererWrapper::OnSurfaceCreated");
}

extern "C"
void
Java_com_imgui_android_exampleapp_RendererWrapper_nativeOnDrawFrame(
        JNIEnv *env,
        jobject /* this */) {
    //LOGI("%s", "RendererWrapper::OnDrawFrame");
	renderFrame();
}

extern "C"
void
Java_com_imgui_android_exampleapp_RendererWrapper_nativeOnSurfaceChanged(
        JNIEnv *env,
        jobject /* this */,
        int width, int height) {
    //LOGI("%s (%d,%d)", "RendererWrapper::OnDrawFrame", width, height);
	screenWidth = width;
	screenHeight = height;
	startTime = currentTimeInMilliseconds();
	setupGraphics(width, height);
}

static void printGLString(const char *name, GLenum s) {
    const char *v = (const char *) glGetString(s);
    LOGI("GL %s = %s\n", name, v);
}

static void checkGlError(const char* op) {
    for (GLint error = glGetError(); error; error
            = glGetError()) {
        LOGI("after %s() glError (0x%x)\n", op, error);
    }
}

auto gVertexShader =
    "attribute vec2 vPosition;\n"
    "void main() {\n"
    "  gl_Position = vec4(vPosition.x, vPosition.y, 0.0, 1.0);\n"
    "}\n";

auto gFragmentShader =
    "precision mediump float;\n"
    "void main() {\n"
    "  gl_FragColor = vec4(0.0, 1.0, 0.0, 1.0);\n"
    "}\n";

GLuint loadShader(GLenum shaderType, const char* pSource) {
    GLuint shader = glCreateShader(shaderType);
    if (shader) {
        glShaderSource(shader, 1, &pSource, NULL);
        glCompileShader(shader);
        GLint compiled = 0;
        glGetShaderiv(shader, GL_COMPILE_STATUS, &compiled);
        if (!compiled) {
            GLint infoLen = 0;
            glGetShaderiv(shader, GL_INFO_LOG_LENGTH, &infoLen);
            if (infoLen) {
                char* buf = (char*) malloc(infoLen);
                if (buf) {
                    glGetShaderInfoLog(shader, infoLen, NULL, buf);
                    LOGE("Could not compile shader %d:\n%s\n",
                            shaderType, buf);
                    free(buf);
                }
                glDeleteShader(shader);
                shader = 0;
            }
        }
    }
    return shader;
}

GLuint createProgram(const char* pVertexSource, const char* pFragmentSource) {
    GLuint vertexShader = loadShader(GL_VERTEX_SHADER, pVertexSource);
    if (!vertexShader) {
        return 0;
    }

    GLuint pixelShader = loadShader(GL_FRAGMENT_SHADER, pFragmentSource);
    if (!pixelShader) {
        return 0;
    }

    GLuint program = glCreateProgram();
    if (program) {
        glAttachShader(program, vertexShader);
        checkGlError("glAttachShader");
        glAttachShader(program, pixelShader);
        checkGlError("glAttachShader");
        glLinkProgram(program);
        GLint linkStatus = GL_FALSE;
        glGetProgramiv(program, GL_LINK_STATUS, &linkStatus);
        if (linkStatus != GL_TRUE) {
            GLint bufLength = 0;
            glGetProgramiv(program, GL_INFO_LOG_LENGTH, &bufLength);
            if (bufLength) {
                char* buf = (char*) malloc(bufLength);
                if (buf) {
                    glGetProgramInfoLog(program, bufLength, NULL, buf);
                    LOGE("Could not link program:\n%s\n", buf);
                    free(buf);
                }
            }
            glDeleteProgram(program);
            program = 0;
        }
    }
    return program;
}

bool setupGraphics(int w, int h) {
	ImGui_ImplAndroidGLES2_Init();
	imClearColor = ImColor(114, 144, 154);
	showTestWindow = true;
    showAnotherWindow = false;
	//glViewport(0, 0, w, h);
    printGLString("Version", GL_VERSION);
    printGLString("Vendor", GL_VENDOR);
    printGLString("Renderer", GL_RENDERER);
    printGLString("Extensions", GL_EXTENSIONS);

    LOGI("setupGraphics(%d, %d)", w, h);
    gProgram = createProgram(gVertexShader, gFragmentShader);
    if (!gProgram) {
        LOGE("Could not create program.");
        return false;
    }
    gvPositionHandle = glGetAttribLocation(gProgram, "vPosition");
    checkGlError("glGetAttribLocation");
    LOGI("glGetAttribLocation(\"vPosition\") = %d\n",
            gvPositionHandle);
	
	glGenBuffers(1, &vbo);
	checkGlError("glGenBuffer");
	glBindBuffer(GL_ARRAY_BUFFER, vbo);
	checkGlError("glBindBuffer");
	glBufferData(GL_ARRAY_BUFFER, 6*sizeof(GLfloat), gTriangleVertices, GL_STATIC_DRAW);
	checkGlError("glBufferData");
	glVertexAttribPointer(gvPositionHandle, 2, GL_FLOAT, GL_FALSE, 0, (GLvoid*)0);
	checkGlError("glVertexAttribPointer");
    glEnableVertexAttribArray(gvPositionHandle);
	checkGlError("glVertexAttribArray");
	glBindBuffer(GL_ARRAY_BUFFER, 0);	
	checkGlError("glBindBuffer");
    glViewport(0, 0, w, h);
    checkGlError("glViewport");
    return true;
}

void renderFrame() {
	ImGui_ImplAndroidGLES2_NewFrame(screenWidth, screenHeight, currentTimeInMilliseconds()-startTime);
    {
		static float f = 0.0f;
		ImGui::SetNextWindowSize(ImVec2(200, 200), ImGuiSetCond_FirstUseEver);
		ImGui::Text("Hello, world!");
		ImGui::SliderFloat("float", &f, 0.0f, 1.0f);
		ImGui::ColorEdit3("clear color", (float *) &imClearColor);
		if (ImGui::Button("Test Window")) showTestWindow = !showTestWindow;
		if (ImGui::Button("Another Window")) showAnotherWindow = !showAnotherWindow;
		ImGui::Text("Application average %.3f ms/frame (%.1f FPS)",
					1000.0f / ImGui::GetIO().Framerate, ImGui::GetIO().Framerate);
	}

	// 2. Show another simple window, this time using an explicit Begin/End pair
	if (showAnotherWindow)
	{
		ImGui::SetNextWindowSize(ImVec2(200,100), ImGuiSetCond_FirstUseEver);
		ImGui::Begin("Another Window", &showAnotherWindow);
		ImGui::Text("Hello");
		ImGui::End();
	}

	// 3. Show the ImGui test window. Most of the sample code is in ImGui::ShowTestWindow()
	if (showTestWindow)
	{
		ImGui::SetNextWindowPos(ImVec2(0, 0), ImGuiSetCond_FirstUseEver);
		ImGui::ShowTestWindow(&showTestWindow);
	}
	
	static float grey;
    grey += 0.01f;
    if (grey > 1.0f) {
        grey = 0.0f;
    }
    glClearColor(grey, grey, grey, 1.0f);
    checkGlError("glClearColor");
    glClear( GL_DEPTH_BUFFER_BIT | GL_COLOR_BUFFER_BIT);
    checkGlError("glClear");
	
	
	
	
    glUseProgram(gProgram);
    checkGlError("glUseProgram");
	
	glBindBuffer(GL_ARRAY_BUFFER, vbo);
	
	//After commentig glVertexAttribArray rendering imgui works fine
	glVertexAttribPointer(gvPositionHandle, 2, GL_FLOAT, GL_FALSE, 0, (GLvoid*)0);
    glEnableVertexAttribArray(gvPositionHandle);
	
    glDrawArrays(GL_TRIANGLES, 0, 3);
    checkGlError("glDrawArrays");
	glBindBuffer(GL_ARRAY_BUFFER, 0);
	ImGui::Render();
}