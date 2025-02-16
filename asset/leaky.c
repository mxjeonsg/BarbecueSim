#include <stdlib.h>

__declspec(dllexport) extern unsigned long* leaky(size_t slots) {
  unsigned long* ptr = (unsigned long*) malloc(sizeof(unsigned long) * slots);

  for(size_t slot = 0; slot <= slots; ++slot) ptr[slot] = 69;

  return ptr;
}