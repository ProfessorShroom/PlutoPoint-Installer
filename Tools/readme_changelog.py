import re
import sys

START = "#### Latest Update"

MAJOR_TITLES = {
    "7": "The .NET 8 / Avalonia Update",
    "6": "The C# Rewrite",
    "5": "The Winget Update",
    "4": "The Dark Update",
    "3": "The NorthPoint Update",
    "2": "The Windows Defender Fix",
    "1": "The Original Release (BRM / CRC Installer)",
}


def parse(text):
    versions = []
    for line in text.splitlines():
        if line.startswith("## "):
            name, _, title = line[3:].strip().partition(" - ")
            versions.append({"name": name, "title": title, "notes": []})
        elif versions and line.startswith("- "):
            versions[-1]["notes"].append(line[2:].strip())
        elif versions and versions[-1]["notes"] and line.startswith("  ") and line.strip():
            versions[-1]["notes"][-1] += "\n" + line
    return versions


def group(versions):
    groups = []
    major = None
    for version in versions:
        numbers = re.match(r"\d+(\.\d+)*$", version["name"])
        if numbers:
            parts = version["name"].split(".")
            major = parts[0]
            key = ".".join(parts[:-1])
        else:
            key = version["name"]
        if groups and groups[-1]["key"] == key and groups[-1]["major"] == major:
            groups[-1]["versions"].append(version)
        else:
            groups.append({"key": key, "major": major, "versions": [version]})
    return groups


def render_group(item):
    versions = item["versions"]
    names = " / ".join(v["name"] for v in versions)
    title = next((v["title"] for v in versions if v["title"]), "")
    numbered = versions[0]["name"][0].isdigit()
    lines = [f"**{'Version ' if numbered else ''}{names}{' - ' + title if title else ''}**", ""]
    for version in versions:
        for note in version["notes"]:
            lines.append(f"- **_{version['name']}_** {note}" if numbered else f"- {note}")
    return "\n".join(lines) + "\n"


def render(versions):
    groups = group(versions)
    titles = dict(MAJOR_TITLES)
    for version in versions:
        match = re.fullmatch(r"(\d+)\.0\.0\.0", version["name"])
        if match and version["title"]:
            titles.setdefault(match[1], version["title"])
    out = ["", "#### Latest Update", "", render_group(groups[0]), "", "#### Older Updates", ""]
    major = None
    for item in groups[1:]:
        if item["major"] != major:
            if major is not None:
                out += ["</details>", ""]
            major = item["major"]
            title = f" - {titles[major]}" if titles.get(major) else ""
            out += ['<details markdown="1">', f"<summary><strong>Version {major}.x{title}</strong></summary>", ""]
        out.append(render_group(item))
    if major is not None:
        out += ["</details>", ""]
    return "\n".join(out)


def main():
    with open("CHANGELOG.md", encoding="utf-8") as file:
        versions = [v for v in parse(file.read()) if not re.search(r"\db\d+$", v["name"])]
    if not versions or not versions[0]["notes"]:
        sys.exit("CHANGELOG.md doesn't have any versions in it")
    with open("README.md", encoding="utf-8", newline="") as file:
        readme = file.read()
    start = readme.find(START)
    if start == -1:
        sys.exit(f"README.md doesn't have a '{START}' heading")
    with open("README.md", "w", encoding="utf-8", newline="\n") as file:
        file.write(readme[:start] + render(versions).strip() + "\n")


if __name__ == "__main__":
    main()
