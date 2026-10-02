const API_URL = "";

const profileIdInput =
    document.getElementById("profileId");

const dateFromInput =
    document.getElementById("dateFrom");

const dateToInput =
    document.getElementById("dateTo");

const searchButton =
    document.getElementById("searchButton");

const clearButton =
    document.getElementById("clearButton");

const message =
    document.getElementById("message");

const resultsSection =
    document.getElementById("resultsSection");

const reportsContainer =
    document.getElementById("reportsContainer");


searchButton.addEventListener(
    "click",
    buscarReportes
);

clearButton.addEventListener(
    "click",
    limpiarBusqueda
);


async function buscarReportes() {
    const profileId =
        profileIdInput.value.trim().toUpperCase();

    const fechaDesde =
        dateFromInput.value;

    const fechaHasta =
        dateToInput.value;


    if (!profileId && !fechaDesde && !fechaHasta) {
        mostrarMensaje(
            "Ingresa un perfil o selecciona un rango de fechas.",
            true
        );

        return;
    }


    if ((fechaDesde && !fechaHasta) ||
        (!fechaDesde && fechaHasta)) {
        mostrarMensaje(
            "Debes seleccionar las dos fechas.",
            true
        );

        return;
    }


    if (fechaDesde &&
        fechaHasta &&
        fechaDesde > fechaHasta) {
        mostrarMensaje(
            "La fecha inicial no puede ser posterior a la fecha final.",
            true
        );

        return;
    }


    mostrarMensaje(
        "Consultando...",
        false
    );

    resultsSection.classList.add("hidden");
    reportsContainer.innerHTML = "";


    try {
        let url;


        // Perfil + fechas
        if (profileId &&
            fechaDesde &&
            fechaHasta) {
            url =
                `${API_URL}/api/reports/date` +
                `?desde=${encodeURIComponent(fechaDesde)}` +
                `&hasta=${encodeURIComponent(fechaHasta)}` +
                `&profileId=${encodeURIComponent(profileId)}`;
        }

        // Solo fechas
        else if (fechaDesde &&
            fechaHasta) {
            url =
                `${API_URL}/api/reports/date` +
                `?desde=${encodeURIComponent(fechaDesde)}` +
                `&hasta=${encodeURIComponent(fechaHasta)}`;
        }

        // Solo perfil
        else {
            url =
                `${API_URL}/api/reports/profile/` +
                encodeURIComponent(profileId);
        }


        const response =
            await fetch(url);


        if (!response.ok) {
            throw new Error(
                "No se pudo consultar el servidor."
            );
        }


        const reports =
            await response.json();


        mostrarReportes(reports);
    }
    catch (error) {
        console.error(error);

        mostrarMensaje(
            "No se pudo conectar con el servidor.",
            true
        );
    }
}


function limpiarBusqueda() {
    profileIdInput.value = "";
    dateFromInput.value = "";
    dateToInput.value = "";

    resultsSection.classList.add("hidden");

    reportsContainer.innerHTML = "";

    mostrarMensaje(
        "",
        false
    );
}

function mostrarReportes(reports) {
    resultsSection.classList.remove("hidden");

    if (reports.length === 0) {
        mostrarMensaje(
            "No se encontraron reportes para este perfil.",
            false
        );

        return;
    }

    mostrarMensaje(
        `Se encontraron ${reports.length} reporte(s).`,
        false
    );

    reports.forEach(report => {
        const card =
            document.createElement("div");

        card.classList.add("report-card");

        card.innerHTML = `
            <h3>Reporte ${report.reportId}</h3>

            <div class="report-data">

                <div>
                    <strong>Perfil</strong>
                    ${report.profileAlias}
                    (${report.profileId})
                </div>

                <div>
                    <strong>Fecha</strong>
                    ${report.fechaPartida}
                </div>

                <div>
                    <strong>Presupuesto inicial</strong>
                    ${formatearDinero(
            report.presupuestoInicial
        )}
                </div>

                <div>
                    <strong>Presupuesto final</strong>
                    ${formatearDinero(
            report.presupuestoFinal
        )}
                </div>

                <div>
                    <strong>Ganancia acumulada</strong>
                    ${formatearDinero(
            report.gananciaAcumulada
        )}
                </div>

                <div>
                    <strong>Gasto acumulado</strong>
                    ${formatearDinero(
            report.gastoAcumulado
        )}
                </div>

                <div>
                    <strong>Periodos jugados</strong>
                    ${report.ciclosJugados}
                </div>

                <div>
                    <strong>Razón de finalización</strong>
                    ${report.razonFin}
                </div>

            </div>
        `;

        reportsContainer.appendChild(card);
    });
}


function formatearDinero(valor) {
    return new Intl.NumberFormat(
        "es-CO",
        {
            style: "currency",
            currency: "COP",
            maximumFractionDigits: 0
        }
    ).format(valor);
}


function mostrarMensaje(texto, esError) {
    message.textContent = texto;

    message.className =
        esError
            ? "message error"
            : "message success";
}
function mostrarReportes(reports) {
    resultsSection.classList.remove("hidden");

    if (reports.length === 0) {
        mostrarMensaje(
            "No se encontraron reportes para este perfil.",
            false
        );

        return;
    }

    mostrarMensaje(
        `Se encontraron ${reports.length} reporte(s).`,
        false
    );

    reports.forEach(report => {
        const card =
            document.createElement("div");

        card.classList.add("report-card");

        card.innerHTML = `
            <h3>Reporte ${report.reportId}</h3>

            <div class="report-data">

                <div>
                    <strong>Perfil</strong>
                    ${report.profileAlias}
                    (${report.profileId})
                </div>

                <div>
                    <strong>Fecha</strong>
                    ${report.fechaPartida}
                </div>

                <div>
                    <strong>Presupuesto inicial</strong>
                    ${formatearDinero(
            report.presupuestoInicial
        )}
                </div>

                <div>
                    <strong>Presupuesto final</strong>
                    ${formatearDinero(
            report.presupuestoFinal
        )}
                </div>

                <div>
                    <strong>Ganancia acumulada</strong>
                    ${formatearDinero(
            report.gananciaAcumulada
        )}
                </div>

                <div>
                    <strong>Gasto acumulado</strong>
                    ${formatearDinero(
            report.gastoAcumulado
        )}
                </div>

                <div>
                    <strong>Periodos jugados</strong>
                    ${report.ciclosJugados}
                </div>

                <div>
                    <strong>Razón de finalización</strong>
                    ${report.razonFin}
                </div>

            </div>

            <div class="periods-section">
                <h4>Historial de periodos</h4>

                <div class="periods-container">
                    ${generarPeriodos(report.historialCiclos)}
                </div>
            </div>
        `;

        reportsContainer.appendChild(card);
    });
}
function generarPeriodos(periodos) {
    if (!periodos || periodos.length === 0) {
        return "<p>No hay información de periodos.</p>";
    }

    return periodos.map((periodo, index) => {
        const numeroPeriodo =
            periodo.numeroCiclo ?? (index + 1);

        return `
            <details class="period-card">

                <summary>
                    Periodo ${numeroPeriodo}
                </summary>

                <div class="period-content">

                    <div class="period-summary">

                        <div>
                            <strong>Presupuesto inicial</strong>
                            ${formatearDinero(
            periodo.presupuestoInicial
        )}
                        </div>

                        <div>
                            <strong>Presupuesto final</strong>
                            ${formatearDinero(
            periodo.presupuestoFinal
        )}
                        </div>

                        <div>
                            <strong>Ganancia</strong>
                            ${formatearDinero(
            periodo.gananciaTotal
        )}
                        </div>

                        <div>
                            <strong>Gastos</strong>
                            ${formatearDinero(
            periodo.gastoTotal
        )}
                        </div>

                    </div>

                    ${generarGastos(periodo.detalleGastos)}

                    ${generarEventos(periodo.eventosOcurridos)}

                    ${generarCosechas(periodo.cosechasRealizadas)}

                    <div class="jornales">
                        <strong>Jornales</strong>

                        <p>
                            Necesarios:
                            ${periodo.jornalesNecesarios}
                        </p>

                        <p>
                            Familiares:
                            ${periodo.jornalesFamiliaresUsados}
                        </p>

                        <p>
                            Contratados:
                            ${periodo.jornalesContratados}
                        </p>
                    </div>

                </div>

            </details>
        `;
    }).join("");
}
function generarGastos(gastos) {
    if (!gastos || gastos.length === 0) {
        return `
            <div class="period-detail">
                <strong>Gastos</strong>
                <p>No hubo gastos registrados.</p>
            </div>
        `;
    }

    return `
        <div class="period-detail">
            <strong>Gastos</strong>

            <ul>
                ${gastos.map(gasto => `
                    <li>
                        ${gasto.descripcion} —
                        ${formatearDinero(gasto.monto)}
                    </li>
                `).join("")}
            </ul>
        </div>
    `;
}


function generarEventos(eventos) {
    if (!eventos || eventos.length === 0) {
        return `
            <div class="period-detail">
                <strong>Eventos</strong>
                <p>No ocurrieron eventos.</p>
            </div>
        `;
    }

    return `
        <div class="period-detail">
            <strong>Eventos</strong>

            <ul>
                ${eventos.map(evento => `
                    <li>${evento}</li>
                `).join("")}
            </ul>
        </div>
    `;
}


function generarCosechas(cosechas) {
    if (!cosechas || cosechas.length === 0) {
        return `
            <div class="period-detail">
                <strong>Cosechas</strong>
                <p>No se realizaron cosechas.</p>
            </div>
        `;
    }

    return `
        <div class="period-detail">
            <strong>Cosechas</strong>

            <ul>
                ${cosechas.map(cosecha => `
                    <li>${cosecha}</li>
                `).join("")}
            </ul>
        </div>
    `;
}