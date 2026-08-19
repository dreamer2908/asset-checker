import React, { useState, useEffect, useRef, useCallback } from 'react';
import axios from 'axios';

function EditAssetModal({ asset, onClose, onSaved }) {
  const [formData, setFormData] = useState({
    資產編號: asset.資產編號 || '',
    資產名稱: asset.資產名稱 || '',
    資產規格: asset.資產規格 || '',
    保管人: asset.保管人 || '',
    姓名: asset.姓名 || '',
    保管代號: asset.保管代號 || '',
    保管人部門: asset.保管人部門 || '',
    放置地點: asset.放置地點 || '',
    供應廠商: asset.供應廠商 || '',
    供應商簡稱: asset.供應商簡稱 || '',
    管理區分: asset.管理區分 || '',
    備註: asset.備註 || '',
  });

  // Custodian dropdown state
  const [custodianQuery, setCustodianQuery] = useState('');
  const [custodianList, setCustodianList] = useState([]);
  const [custodianPage, setCustodianPage] = useState(1);
  const [hasMoreCustodians, setHasMoreCustodians] = useState(true);
  const [isLoadingCustodians, setIsLoadingCustodians] = useState(false);
  const [showCustodianDropdown, setShowCustodianDropdown] = useState(false);

  // Department dropdown state
  const [deptQuery, setDeptQuery] = useState('');
  const [deptList, setDeptList] = useState([]);
  const [deptPage, setDeptPage] = useState(1);
  const [hasMoreDepts, setHasMoreDepts] = useState(true);
  const [isLoadingDepts, setIsLoadingDepts] = useState(false);
  const [showDeptDropdown, setShowDeptDropdown] = useState(false);

  // Location bookmarks state
  const [locationBookmarks, setLocationBookmarks] = useState([]);
  const [showLocationDropdown, setShowLocationDropdown] = useState(false);

  const [errorMsg, setErrorMsg] = useState('');
  const [isSaving, setIsSaving] = useState(false);

  const custodianDropdownRef = useRef(null);
  const deptDropdownRef = useRef(null);
  const locationDropdownRef = useRef(null);

  // Fetch custodian detail when 保管人 value changes
  const fetchCustodianDetail = async (code) => {
    if (!code || !code.trim()) {
      setFormData((prev) => ({
        ...prev,
        姓名: '',
        保管人部門: '',
      }));
      return;
    }
    try {
      const res = await axios.get(`/api/custodians/details/${encodeURIComponent(code.trim())}`);
      if (res.data) {
        setFormData((prev) => ({
          ...prev,
          保管人: res.data.保管人 || prev.保管人,
          姓名: res.data.姓名 || '',
          保管代號: res.data.保管代號 || prev.保管代號,
          保管人部門: res.data.保管人部門 || '',
        }));
      }
    } catch (err) {
      setFormData((prev) => ({
        ...prev,
        姓名: '',
        保管人部門: '',
      }));
    }
  };

  // Fetch custodians page
  const fetchCustodians = useCallback(async (q, deptCode, pageNum, reset = false) => {
    setIsLoadingCustodians(true);
    try {
      const res = await axios.get('/api/custodians', {
        params: { q, deptCode, page: pageNum, pageSize: 20 },
      });
      const newItems = res.data || [];
      if (newItems.length < 20) {
        setHasMoreCustodians(false);
      } else {
        setHasMoreCustodians(true);
      }

      setCustodianList((prev) => {
        if (reset || pageNum === 1) {
          return newItems;
        }
        const existingCodes = new Set(prev.map((item) => item.保管人));
        const filteredNew = newItems.filter((item) => !existingCodes.has(item.保管人));
        return [...prev, ...filteredNew];
      });
    } catch (err) {
      console.error('Error fetching custodians:', err);
    } finally {
      setIsLoadingCustodians(false);
    }
  }, []);

  // Fetch departments page
  const fetchDepartments = useCallback(async (q, pageNum, reset = false) => {
    setIsLoadingDepts(true);
    try {
      const res = await axios.get('/api/departments', {
        params: { q, page: pageNum, pageSize: 20 },
      });
      const newItems = res.data || [];
      if (newItems.length < 20) {
        setHasMoreDepts(false);
      } else {
        setHasMoreDepts(true);
      }

      setDeptList((prev) => {
        if (reset || pageNum === 1) {
          return newItems;
        }
        const existingCodes = new Set(prev.map((item) => item.保管代號));
        const filteredNew = newItems.filter((item) => !existingCodes.has(item.保管代號));
        return [...prev, ...filteredNew];
      });
    } catch (err) {
      console.error('Error fetching departments:', err);
    } finally {
      setIsLoadingDepts(false);
    }
  }, []);

  // Fetch location bookmarks
  const fetchLocationBookmarks = useCallback(async () => {
    try {
      const res = await axios.get('/api/bookmarks/locations');
      setLocationBookmarks(res.data || []);
    } catch (err) {
      console.error('Error fetching location bookmarks:', err);
    }
  }, []);

  // Handle outside click to close dropdowns
  useEffect(() => {
    const handleClickOutside = (e) => {
      if (custodianDropdownRef.current && !custodianDropdownRef.current.contains(e.target)) {
        setShowCustodianDropdown(false);
      }
      if (deptDropdownRef.current && !deptDropdownRef.current.contains(e.target)) {
        setShowDeptDropdown(false);
      }
      if (locationDropdownRef.current && !locationDropdownRef.current.contains(e.target)) {
        setShowLocationDropdown(false);
      }
    };
    document.addEventListener('mousedown', handleClickOutside);
    return () => document.removeEventListener('mousedown', handleClickOutside);
  }, []);

  // Initial fetch for dropdowns
  useEffect(() => {
    setCustodianPage(1);
    fetchCustodians(custodianQuery, formData.保管代號, 1, true);
  }, [custodianQuery, formData.保管代號, fetchCustodians]);

  useEffect(() => {
    setDeptPage(1);
    fetchDepartments(deptQuery, 1, true);
  }, [deptQuery, fetchDepartments]);

  useEffect(() => {
    fetchLocationBookmarks();
  }, [fetchLocationBookmarks]);

  // Lazy load custodians on scroll
  const handleCustodianScroll = (e) => {
    const { scrollTop, clientHeight, scrollHeight } = e.target;
    if (scrollTop + clientHeight >= scrollHeight - 15 && hasMoreCustodians && !isLoadingCustodians) {
      const nextPage = custodianPage + 1;
      setCustodianPage(nextPage);
      fetchCustodians(custodianQuery, formData.保管代號, nextPage, false);
    }
  };

  // Lazy load departments on scroll
  const handleDeptScroll = (e) => {
    const { scrollTop, clientHeight, scrollHeight } = e.target;
    if (scrollTop + clientHeight >= scrollHeight - 15 && hasMoreDepts && !isLoadingDepts) {
      const nextPage = deptPage + 1;
      setDeptPage(nextPage);
      fetchDepartments(deptQuery, nextPage, false);
    }
  };

  // Handle Custodian input change
  const handleCustodianInputChange = (e) => {
    const val = e.target.value;
    setFormData((prev) => ({ ...prev, 保管人: val }));
    setCustodianQuery(val);
    setShowCustodianDropdown(true);
    if (val.trim()) {
      fetchCustodianDetail(val);
    } else {
      setFormData((prev) => ({ ...prev, 姓名: '', 保管人部門: '' }));
    }
  };

  // Select Custodian from dropdown
  const selectCustodian = (item) => {
    setFormData((prev) => ({
      ...prev,
      保管人: item.保管人,
      姓名: item.姓名,
      保管代號: item.保管代號 || prev.保管代號,
      保管人部門: item.保管人部門,
    }));
    setShowCustodianDropdown(false);
  };

  // Bookmark toggles
  const toggleCustodianBookmark = async (e, code) => {
    e.stopPropagation();
    try {
      await axios.post('/api/bookmarks/toggle', { custodianCode: code });
      fetchCustodians(custodianQuery, formData.保管代號, 1, true);
    } catch (err) {
      console.error('Error toggling custodian bookmark:', err);
    }
  };

  const toggleDeptBookmark = async (e, code) => {
    e.stopPropagation();
    try {
      await axios.post('/api/bookmarks/toggle-dept', { custodianCode: code });
      fetchDepartments(deptQuery, 1, true);
    } catch (err) {
      console.error('Error toggling department bookmark:', err);
    }
  };

  const toggleLocationBookmark = async (loc) => {
    if (!loc || !loc.trim()) return;
    try {
      const res = await axios.post('/api/bookmarks/toggle-location', { custodianCode: loc.trim() });
      setLocationBookmarks(res.data.bookmarks || []);
    } catch (err) {
      console.error('Error toggling location bookmark:', err);
    }
  };

  const isLocationBookmarked = (loc) => {
    if (!loc || !loc.trim()) return false;
    return locationBookmarks.some((b) => b.toLowerCase() === loc.trim().toLowerCase());
  };

  // Handle Department input change
  const handleDeptInputChange = (e) => {
    const val = e.target.value;
    setFormData((prev) => ({ ...prev, 保管代號: val }));
    setDeptQuery(val);
    setShowDeptDropdown(true);
  };

  // Select Department from dropdown
  const selectDepartment = (item) => {
    setFormData((prev) => ({
      ...prev,
      保管代號: item.保管代號,
      保管人部門: item.保管人部門 || prev.保管人部門,
    }));
    setShowDeptDropdown(false);
  };

  // Save handler
  const handleSave = async () => {
    setErrorMsg('');
    if (!formData.保管人.trim()) {
      setErrorMsg('請輸入或選擇有效的保管人');
      return;
    }
    if (!formData.保管代號.trim()) {
      setErrorMsg('請輸入或選擇有效的保管代號');
      return;
    }

    setIsSaving(true);
    try {
      await axios.put(`/api/assets/${encodeURIComponent(formData.資產編號)}`, {
        保管人: formData.保管人,
        保管代號: formData.保管代號,
        放置地點: formData.放置地點,
        備註: formData.備註,
      });
      setIsSaving(false);
      onSaved();
    } catch (err) {
      setIsSaving(false);
      if (err.response && err.response.data && err.response.data.message) {
        setErrorMsg(err.response.data.message);
      } else {
        setErrorMsg('儲存時發生錯誤，請檢查輸入資料');
      }
    }
  };

  return (
    <div className="modal show d-block" tabIndex="-1" style={{ backgroundColor: 'rgba(0,0,0,0.5)' }}>
      <div className="modal-dialog modal-lg modal-dialog-scrollable">
        <div className="modal-content shadow-lg">
          <div className="modal-header bg-primary text-white">
            <h5 className="modal-title">編輯資產保管資料 – {formData.資產編號}</h5>
            <button type="button" className="btn-close btn-close-white" onClick={onClose}></button>
          </div>
          <div className="modal-body">
            {errorMsg && (
              <div className="alert alert-danger py-2 mb-3" role="alert">
                ⚠️ {errorMsg}
              </div>
            )}

            <div className="row g-3">
              {/* Read-Only Fields Row 1 */}
              <div className="col-md-4">
                <label className="form-label text-muted fw-bold">資產編號 (唯讀)</label>
                <input type="text" className="form-control bg-light" value={formData.資產編號} readOnly />
              </div>

              <div className="col-md-4">
                <label className="form-label text-muted fw-bold">資產名稱 (唯讀)</label>
                <input type="text" className="form-control bg-light" value={formData.資產名稱} readOnly />
              </div>

              <div className="col-md-4">
                <label className="form-label text-muted fw-bold">資產規格 (唯讀)</label>
                <input type="text" className="form-control bg-light" value={formData.資產規格} readOnly />
              </div>

              {/* Editable Custodian Row 2 */}
              <div className="col-md-4 position-relative" ref={custodianDropdownRef}>
                <label className="form-label fw-bold text-primary">
                  保管人 <span className="text-danger">*</span>
                </label>
                <div className="input-group">
                  <input
                    type="text"
                    className="form-control"
                    placeholder="搜尋或輸入保管人代碼"
                    value={formData.保管人}
                    onChange={handleCustodianInputChange}
                    onFocus={() => {
                      setShowCustodianDropdown(true);
                      fetchCustodians(formData.保管人, formData.保管代號, 1, true);
                    }}
                  />
                  {formData.保管人 && (
                    <button
                      className="btn btn-outline-secondary"
                      type="button"
                      title="清除保管人"
                      onClick={() => {
                        setFormData((prev) => ({ ...prev, 保管人: '', 姓名: '', 保管人部門: '' }));
                        setCustodianQuery('');
                      }}
                    >
                      ✕
                    </button>
                  )}
                  <button
                    className="btn btn-outline-secondary dropdown-toggle"
                    type="button"
                    onClick={() => {
                      setShowCustodianDropdown(!showCustodianDropdown);
                      fetchCustodians(custodianQuery, formData.保管代號, 1, true);
                    }}
                  ></button>
                </div>

                {/* Custodian Dropdown List */}
                {showCustodianDropdown && (
                  <div
                    className="dropdown-menu show w-100 shadow-sm"
                    onScroll={handleCustodianScroll}
                    style={{
                      maxHeight: '240px',
                      overflowY: 'auto',
                      position: 'absolute',
                      zIndex: 1050,
                      top: '100%',
                    }}
                  >
                    {custodianList.length === 0 && !isLoadingCustodians ? (
                      <div className="dropdown-item text-muted small py-2">無符合資料</div>
                    ) : (
                      custodianList.map((c, i) => (
                        <div
                          key={i}
                          className={`dropdown-item d-flex justify-content-between align-items-center py-2 ${
                            c.isBookmarked ? 'bg-warning-subtle fw-bold' : ''
                          }`}
                          style={{ cursor: 'pointer' }}
                          onClick={() => selectCustodian(c)}
                        >
                          <div>
                            <span className="fw-bold me-2">{c.保管人}</span>
                            <span className="text-dark me-2">{c.姓名}</span>
                            <small className="text-muted">({c.保管人部門 || '無部門'})</small>
                          </div>
                          <button
                            type="button"
                            className={`btn btn-sm ${c.isBookmarked ? 'btn-warning text-dark' : 'btn-outline-secondary'}`}
                            style={{ padding: '0px 6px', fontSize: '14px' }}
                            title={c.isBookmarked ? '取消書籤' : '加入書籤'}
                            onClick={(e) => toggleCustodianBookmark(e, c.保管人)}
                          >
                            {c.isBookmarked ? '★' : '☆'}
                          </button>
                        </div>
                      ))
                    )}
                    {isLoadingCustodians && (
                      <div className="dropdown-item text-center text-muted small py-2">載入中...</div>
                    )}
                  </div>
                )}
              </div>

              {/* Read-Only Custodian Name */}
              <div className="col-md-4">
                <label className="form-label text-muted fw-bold">姓名 (唯讀)</label>
                <input type="text" className="form-control bg-light" value={formData.姓名} readOnly />
              </div>

              {/* Editable Department Code */}
              <div className="col-md-4 position-relative" ref={deptDropdownRef}>
                <label className="form-label fw-bold text-primary">
                  保管代號 <span className="text-danger">*</span>
                </label>
                <div className="input-group">
                  <input
                    type="text"
                    className="form-control"
                    placeholder="搜尋或輸入保管代號"
                    value={formData.保管代號}
                    onChange={handleDeptInputChange}
                    onFocus={() => {
                      setShowDeptDropdown(true);
                      fetchDepartments(formData.保管代號, 1, true);
                    }}
                  />
                  {formData.保管代號 && (
                    <button
                      className="btn btn-outline-secondary"
                      type="button"
                      title="清除保管代號"
                      onClick={() => {
                        setFormData((prev) => ({ ...prev, 保管代號: '' }));
                        setDeptQuery('');
                      }}
                    >
                      ✕
                    </button>
                  )}
                  <button
                    className="btn btn-outline-secondary dropdown-toggle"
                    type="button"
                    onClick={() => {
                      setShowDeptDropdown(!showDeptDropdown);
                      fetchDepartments(deptQuery, 1, true);
                    }}
                  ></button>
                </div>

                {/* Department Dropdown List */}
                {showDeptDropdown && (
                  <div
                    className="dropdown-menu show w-100 shadow-sm"
                    onScroll={handleDeptScroll}
                    style={{
                      maxHeight: '240px',
                      overflowY: 'auto',
                      position: 'absolute',
                      zIndex: 1050,
                      top: '100%',
                    }}
                  >
                    {deptList.length === 0 && !isLoadingDepts ? (
                      <div className="dropdown-item text-muted small py-2">無符合資料</div>
                    ) : (
                      deptList.map((d, i) => (
                        <div
                          key={i}
                          className={`dropdown-item d-flex justify-content-between align-items-center py-2 ${
                            d.isBookmarked ? 'bg-warning-subtle fw-bold' : ''
                          }`}
                          style={{ cursor: 'pointer' }}
                          onClick={() => selectDepartment(d)}
                        >
                          <div>
                            <span className="fw-bold me-2">{d.保管代號}</span>
                            <span className="text-muted">{d.保管人部門}</span>
                          </div>
                          <button
                            type="button"
                            className={`btn btn-sm ${d.isBookmarked ? 'btn-warning text-dark' : 'btn-outline-secondary'}`}
                            style={{ padding: '0px 6px', fontSize: '14px' }}
                            title={d.isBookmarked ? '取消書籤' : '加入書籤'}
                            onClick={(e) => toggleDeptBookmark(e, d.保管代號)}
                          >
                            {d.isBookmarked ? '★' : '☆'}
                          </button>
                        </div>
                      ))
                    )}
                    {isLoadingDepts && (
                      <div className="dropdown-item text-center text-muted small py-2">載入中...</div>
                    )}
                  </div>
                )}
              </div>

              {/* Read-Only Department Name Row 3 (col-md-4) */}
              <div className="col-md-4">
                <label className="form-label text-muted fw-bold">保管人部門 (唯讀)</label>
                <input type="text" className="form-control bg-light" value={formData.保管人部門} readOnly />
              </div>

              {/* Editable Location Row 3 (Spans 2 columns: col-md-8) */}
              <div className="col-md-8 position-relative" ref={locationDropdownRef}>
                <label className="form-label fw-bold text-primary">放置地點</label>
                <div className="input-group">
                  <input
                    type="text"
                    className="form-control"
                    placeholder="輸入或選擇放置地點"
                    value={formData.放置地點}
                    onChange={(e) => setFormData({ ...formData, 放置地點: e.target.value })}
                    onFocus={() => setShowLocationDropdown(true)}
                  />
                  {formData.放置地點 && (
                    <button
                      className="btn btn-outline-secondary"
                      type="button"
                      title="清除放置地點"
                      onClick={() => setFormData({ ...formData, 放置地點: '' })}
                    >
                      ✕
                    </button>
                  )}
                  {formData.放置地點.trim() && (
                    <button
                      className={`btn ${isLocationBookmarked(formData.放置地點) ? 'btn-warning text-dark' : 'btn-outline-secondary'}`}
                      type="button"
                      title={isLocationBookmarked(formData.放置地點) ? '取消放置地點書籤' : '加入放置地點書籤'}
                      onClick={() => toggleLocationBookmark(formData.放置地點)}
                    >
                      {isLocationBookmarked(formData.放置地點) ? '★' : '☆'}
                    </button>
                  )}
                  <button
                    className="btn btn-outline-secondary dropdown-toggle"
                    type="button"
                    title="常用放置地點選單"
                    onClick={() => setShowLocationDropdown(!showLocationDropdown)}
                  ></button>
                </div>

                {/* Location Bookmarks Dropdown */}
                {showLocationDropdown && (
                  <div
                    className="dropdown-menu show w-100 shadow-sm"
                    style={{
                      maxHeight: '240px',
                      overflowY: 'auto',
                      position: 'absolute',
                      zIndex: 1050,
                      top: '100%',
                    }}
                  >
                    <div className="dropdown-header fw-bold text-dark border-bottom py-1">常用放置地點書籤</div>
                    {locationBookmarks.length === 0 ? (
                      <div className="dropdown-item text-muted small py-2">尚無放置地點書籤</div>
                    ) : (
                      locationBookmarks.map((loc, i) => (
                        <div
                          key={i}
                          className="dropdown-item d-flex justify-content-between align-items-center py-2"
                          style={{ cursor: 'pointer' }}
                          onClick={() => {
                            setFormData((prev) => ({ ...prev, 放置地點: loc }));
                            setShowLocationDropdown(false);
                          }}
                        >
                          <span className="fw-bold me-2">{loc}</span>
                          <button
                            type="button"
                            className="btn btn-sm btn-warning text-dark"
                            style={{ padding: '0px 6px', fontSize: '12px' }}
                            title="取消書籤"
                            onClick={(e) => {
                              e.stopPropagation();
                              toggleLocationBookmark(loc);
                            }}
                          >
                            ★
                          </button>
                        </div>
                      ))
                    )}
                  </div>
                )}
              </div>

              {/* Read-Only Supplier Codes Row 4 */}
              <div className="col-md-4">
                <label className="form-label text-muted fw-bold">供應廠商 (唯讀)</label>
                <input type="text" className="form-control bg-light" value={formData.供應廠商} readOnly />
              </div>

              <div className="col-md-4">
                <label className="form-label text-muted fw-bold">供應商簡稱 (唯讀)</label>
                <input type="text" className="form-control bg-light" value={formData.供應商簡稱} readOnly />
              </div>

              <div className="col-md-4">
                <label className="form-label text-muted fw-bold">管理區分 (唯讀)</label>
                <input type="text" className="form-control bg-light" value={formData.管理區分} readOnly />
              </div>

              {/* Editable Remarks Row 5 */}
              <div className="col-12">
                <label className="form-label fw-bold text-primary">備註</label>
                <textarea
                  className="form-control"
                  rows="3"
                  placeholder="輸入備註說明"
                  value={formData.備註}
                  onChange={(e) => setFormData({ ...formData, 備註: e.target.value })}
                ></textarea>
              </div>
            </div>
          </div>

          <div className="modal-footer bg-light">
            <button type="button" className="btn btn-secondary" onClick={onClose}>
              取消
            </button>
            <button type="button" className="btn btn-primary" onClick={handleSave} disabled={isSaving}>
              {isSaving ? '儲存中...' : '儲存'}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default EditAssetModal;
