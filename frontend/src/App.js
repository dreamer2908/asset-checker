import React, { useState, useEffect, useCallback } from 'react';
import axios from 'axios';
import EditAssetModal from './EditAssetModal';

function App() {
  const [assets, setAssets] = useState([]);
  const [managerType, setManagerType] = useState('');
  const [page, setPage] = useState(1);
  const [pageSize] = useState(20);
  const [total, setTotal] = useState(0);
  const [assetId, setAssetId] = useState('');

  const [editingAsset, setEditingAsset] = useState(null);

  const fetchAssets = useCallback(async () => {
    try {
      const res = await axios.get(`/assets`, {
        params: { managerType, page, pageSize, assetId }
      });
      setAssets(res.data.data);
      setTotal(res.data.total);
    } catch (err) {
      console.error(err);
    }
  }, [managerType, page, pageSize, assetId]);

  useEffect(() => {
    fetchAssets();
  }, [fetchAssets]);

  const totalPages = Math.ceil(total / pageSize) || 1;

  const columnHeaders = [
    "操作", "資產編號", "資產名稱", "資產規格", "保管人", "姓名", "保管代號",
    "保管人部門", "放置地點", "供應廠商", "供應商簡稱",
    "管理區分", "備註"
  ];

  return (
    <div style={{ padding: 20 }}>
      <div className="navbar">
        <div className="nav-logo d-flex align-items-center">
          <a href="/assets">
            <img alt="Logo" className="logo-image me-2" src="/logo-removebg-preview.a734f6a9c60e6d061827.jpg" />
          </a>
        </div>
      </div>
      <h1>資產管理 – 查詢頁面</h1>

      {/* Dropdown + Export button trên cùng 1 hàng */}
      <div className="row align-items-end mb-3">
        {/* Dropdown 管理區分 */}
        <div className="col-md-4">
          <label className="form-label">管理區分:</label>
          <select
            className="form-select"
            value={managerType}
            onChange={(e) => {
              setPage(1);
              setManagerType(e.target.value);
            }}
          >
            <option value="">-- Tất cả --</option>
            <option value="G">Tổng Vụ - 總務</option>
            <option value="M">Sản xuất - 生產</option>
            <option value="L">Lab - 實驗室</option>
            <option value="I">IT - 資訊部</option>
            <option value="K">Khai Phá - 開發樣品</option>
          </select>
        </div>

        {/* Tìm kiếm theo 資產編號 */}
        <div className="col-md-4">
          <label className="form-label">資產編號:</label>
          <input
            type="text"
            className="form-control"
            placeholder="輸入資產編號"
            value={assetId}
            onChange={(e) => {
              setPage(1);
              setAssetId(e.target.value);
            }}
          />
        </div>

        {/* Nút Export Excel */}
        <div className="col-md-2">
          <button
            className="btn btn-success mt-3"
            onClick={() => {
              const link = document.createElement('a');
              let query = [];
              if (managerType) query.push(`managerType=${encodeURIComponent(managerType)}`);
              if (assetId) query.push(`assetId=${encodeURIComponent(assetId)}`);
              const queryString = query.length > 0 ? `?${query.join('&')}` : '';
              link.href = `/api/export${queryString}`;
              link.setAttribute('download', `assets_${managerType || 'ALL'}.xlsx`);
              document.body.appendChild(link);
              link.click();
              document.body.removeChild(link);
            }}
          >
            匯出 Excel
          </button>
        </div>
      </div>

      {/* Bảng dữ liệu */}
      <div className="table-responsive">
        <table className="table table-primary table-bordered table-striped align-middle">
          <thead>
            <tr>
              {columnHeaders.map(h => <th key={h}>{h}</th>)}
            </tr>
          </thead>
          <tbody>
            {assets.map((row, i) => (
              <tr key={i}>
                <td>
                  <button
                    className="btn btn-sm btn-primary"
                    onClick={() => setEditingAsset(row)}
                  >
                    編輯
                  </button>
                </td>
                <td>{row.資產編號}</td>
                <td>{row.資產名稱}</td>
                <td>{row.資產規格}</td>
                <td>{row.保管人}</td>
                <td>{row.姓名}</td>
                <td>{row.保管代號}</td>
                <td>{row.保管人部門}</td>
                <td>{row.放置地點}</td>
                <td>{row.供應廠商}</td>
                <td>{row.供應商簡稱}</td>
                <td>{row.管理區分}</td>
                <td>{row.備註}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      {/* Phân trang */}
      <div className="d-flex align-items-center mt-3">
        <button className="btn btn-outline-primary me-2" onClick={() => setPage(p => Math.max(1, p - 1))} disabled={page === 1}>
          ⬅️ Trang trước 上一頁
        </button>
        <span className="me-2">Trang {page} / {totalPages}</span>
        <button className="btn btn-outline-primary" onClick={() => setPage(p => Math.min(totalPages, p + 1))} disabled={page === totalPages}>
          ➡️ Trang sau 下一頁
        </button>
      </div>

      {/* Edit Modal */}
      {editingAsset && (
        <EditAssetModal
          asset={editingAsset}
          onClose={() => setEditingAsset(null)}
          onSaved={() => {
            setEditingAsset(null);
            fetchAssets();
          }}
        />
      )}
    </div>
  );
}

export default App;
