# user configuration
version:=1.0
revision:=1
reference=

shell_executor=pwsh -c
just=just

# .PHONY := build, test

.DEFAULT_GOAL := build

# install:
# 	$(shell_executor) $(just) install

build:
	$(shell_executor) $(just) build

test:
	$(shell_executor) $(just) test

# publish:
# 	$(shell_executor) $(just) publish

# doc_publish:
# 	$(shell_executor) $(just) doc_publish
# doc_watch:
# 	$(shell_executor) $(just) doc_watch
