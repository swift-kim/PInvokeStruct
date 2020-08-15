#include <stdio.h>

typedef struct {
    const char** switches;
    size_t switches_count;
} Properties;

extern "C" void NativeFoo(const Properties &properties)
{
    printf("first: %s\n", properties.switches[0]);
}
