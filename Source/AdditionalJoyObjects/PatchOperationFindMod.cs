using System.Linq;
using System.Xml;

using Verse;

namespace AdditionalJoyObjects {

  internal class PatchOperationFindMod : PatchOperation {

    private string modName;

    protected override bool ApplyWorker(XmlDocument xml) {
      if (modName.NullOrEmpty()) {
        return false;
      }
      return ModsConfig.ActiveModsInLoadOrder.Any(m => m.Name == modName);
    }
  }
}
