#include <m.h>

/*
==================
main
==================
*/
int
main (int argc, char *argv[])
{
	// Initialize Mono Domain
	MDomain *const domain = m_domain_new ("Mono\\lib", "Mono\\etc", "FQuake3", M_RUNTIME_4_5);

	// Load Engine Assembly
	m_load_assembly ("Engine.dll");

	// Invoke Engine Init
	m_invoke_module_function ("Engine", "Engine", "System", "Init", NULL);

	// Free Domain
	m_domain_free (domain);

	return 0;
}