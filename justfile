# use PowerShell instead of sh:
set shell := ["pwsh", "-ExecutionPolicy", "bypass", "-c"]

scriptsDir:=    "./scripts"
# installScript:= join(scriptsDir, "install.ps1")
buildScript:=   join(scriptsDir, "build.ps1")
testScript:=    join(scriptsDir, "test.ps1")
# releaseScript:= join(scriptsDir, "release.ps1")
# docScript:=     join(scriptsDir, "doc.ps1")

# default recipe to display help information
default:
  @just --list

#
# install
#
# install:
#   @just install_doctest

# install_doctest:
#   & { . {{ installScript }}; Install-Doctest }

#
# build
#
build:
  @just build_sample

build_sample:
  & { . {{ buildScript }}; Build-TestServer }

#
# test
#
test:
  @just test_robotframework

test_robotframework:
  & { . {{ testScript }}; Test-RobotFramework }

#
# publish
#
# publish:
#   @just release_publish

# release_publish:
#   & { . {{ releaseScript }}; Publish }

#
# doc
#
# doc_publish:
#   @just doc_publish

# doc_publish:
#   & { . {{ docScript }}; Publish }

# doc_watch:
#   watchexec -w ./doc/presentation --exts md just doc_publish
