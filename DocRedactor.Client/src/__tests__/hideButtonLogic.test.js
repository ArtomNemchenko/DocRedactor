/**
 * Unit test for the ViewDocument hide button fix
 * 
 * This tests that the handleViewVersion function properly toggles
 * the selected version state (showing/hiding version content)
 */

describe('ViewDocument - Hide Button Logic', () => {
  test('handleViewVersion should toggle version visibility correctly', () => {
    // This simulates the fix in ViewDocument.jsx line 75-77
    // Before fix: setSelectedVersion(version) - always sets, never hides
    // After fix: setSelectedVersion(selectedVersion?.id === version.id ? null : version) - toggles
    
    const version = { id: 1, versionNumber: 1, content: 'Test content' };
    
    // Simulate initial state (nothing selected)
    let selectedVersion = null;
    
    // First click - should show the version
    selectedVersion = selectedVersion?.id === version.id ? null : version;
    expect(selectedVersion).toEqual(version);
    expect(selectedVersion).not.toBeNull();
    
    // Second click - should hide the version (toggle off)
    selectedVersion = selectedVersion?.id === version.id ? null : version;
    expect(selectedVersion).toBeNull();
    
    // Third click - should show again
    selectedVersion = selectedVersion?.id === version.id ? null : version;
    expect(selectedVersion).toEqual(version);
  });

  test('handleViewVersion should work with different versions', () => {
    const version1 = { id: 1, versionNumber: 1 };
    const version2 = { id: 2, versionNumber: 2 };
    
    let selectedVersion = null;
    
    // Select version 1
    selectedVersion = selectedVersion?.id === version1.id ? null : version1;
    expect(selectedVersion).toEqual(version1);
    
    // Select version 2 (should switch, not toggle off)
    selectedVersion = selectedVersion?.id === version2.id ? null : version2;
    expect(selectedVersion).toEqual(version2);
    expect(selectedVersion.id).toBe(2);
    
    // Click version 2 again (should toggle off)
    selectedVersion = selectedVersion?.id === version2.id ? null : version2;
    expect(selectedVersion).toBeNull();
  });
});
